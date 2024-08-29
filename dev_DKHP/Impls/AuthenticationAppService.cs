using dev_DKHP.CoreModule.Const;
using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Helper.Authorization;
using dev_DKHP.CoreModule.Helper.EmailSender;
using dev_DKHP.CoreModule.Model;
using dev_DKHP.Intfs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace dev_DKHP.Impls
{
    public class AuthenticationAppService : IAuthenticationAppService
    {
        private readonly DKHPDbContext _dbContext;
        private readonly UserManager<TL_USER_ENTITY> _userManager;
        private readonly RoleManager<IdentityRole<string>> _roleManager;
        private readonly IEmailAppService _emailAppService;
        public AuthenticationAppService(
            DKHPDbContext dbContext,
            UserManager<TL_USER_ENTITY> userManager,
            RoleManager<IdentityRole<string>> roleManager,
            IEmailAppService emailAppService
            )
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailAppService = emailAppService;
        }
        public async Task<CommonReturnDto> CreateUserAsync(TL_USER_ENTITY user)
        {
            try
            {
                var checkUser = await _userManager.FindByEmailAsync(user.Email);
                if (checkUser != null)
                {
                    throw new CustomException(-1, "Email already exists", user);
                }

                checkUser = await _userManager.FindByNameAsync(user.UserName);
                if (checkUser != null)
                {
                    throw new CustomException(-1, "Username already exists", user);
                }
                var savedUser = new TL_USER_ENTITY
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PASSWORD = user.PASSWORD,
                    USER_CODE = user.USER_CODE,
                    DEP_ID = user.DEP_ID,
                    NormalizedUserName = user.NormalizedUserName,
                    NormalizedEmail = user.NormalizedEmail,
                    EmailConfirmed = user.EmailConfirmed,
                    SecurityStamp = user.SecurityStamp,
                    ConcurrencyStamp = user.ConcurrencyStamp,
                    PhoneNumber = user.PhoneNumber,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    AccessFailedCount = user.AccessFailedCount
                };
                var result = await _userManager.CreateAsync(savedUser, savedUser.PASSWORD);
                if (result.Succeeded)
                {
                    savedUser = await _userManager.FindByNameAsync(savedUser.UserName);
                    var roles = user.ROLES;
                    foreach (var role in roles)
                    {
                        var checkVaidRole = await _roleManager.FindByIdAsync(role.RoleId);
                        if (checkVaidRole != null)
                        {
                            var userRole = new IdentityUserRole<string>
                            {
                                UserId = savedUser.Id,
                                RoleId = checkVaidRole.Id
                            };
                            _dbContext.UserRoles.Add(userRole);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                    return new CommonReturnDto
                    {
                        STATUS_CODE = 0,
                        ERROR_MESSAGE = "Successfully",
                        DATA = user
                    };
                }
                else throw new CustomException(-1, "Failure", user);
            }
            catch (CustomException ex)
            {
                throw new CustomException(-1, ex.Message);
            }

        }
        public async Task<CommonReturnDto> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) throw new CustomException(-1, "User not found");

            var isValidPassword = await _userManager.CheckPasswordAsync(user, oldPassword);
            if (!isValidPassword) throw new CustomException(-1, "Invalid Password");

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (result.Succeeded)
                return new CommonReturnDto
                {
                    STATUS_CODE = 0,
                    ERROR_MESSAGE = "Change password successfully"
                };
            else throw new CustomException(-1, "Change password failed");
        }
        public async Task<CommonReturnDto> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) throw new CustomException(-1, "User not found");

                // Generate otp
                string otpCode;
                do
                {
                    // Kiểm tra nếu trùng mã otp thì tạo otp mới
                    otpCode = (new Random()).Next(100000, 999999).ToString();
                    var existsOtp = _dbContext.EmailSenderEntities
                        .Where(e => e.RECEIVER_EMAIL == email && e.TYPE_EMAIL == TypeEmailConst.RECOVERY_PASSORD && e.CONTENT == otpCode)
                        .FirstOrDefault();
                    if (existsOtp == null) break;
                } while (true);

                // START SAVE EMAIL ENTITY
                await _dbContext.EmailSenderEntities
                    .Where(e => e.RECEIVER_EMAIL == email && e.TYPE_EMAIL == TypeEmailConst.RECOVERY_PASSORD && e.IS_VALID == true)
                    .ExecuteUpdateAsync(f => f.SetProperty(x => x.IS_VALID, false));

                var EMAIL_ENTITY = new EMAIL_SENDER_ENTITY
                {
                    EMAIL_ID = Guid.NewGuid().ToString() + DateTime.Now.ToString(),
                    RECEIVER_EMAIL = email,
                    CONTENT = otpCode,
                    TYPE_EMAIL = TypeEmailConst.RECOVERY_PASSORD,
                    SUBJECT = TypeEmailConst.RECOVERY_PASSORD,
                    IS_VALID = true,
                    CREATE_DT = DateTime.Now,
                    EXPIRE_DT = DateTime.Now.AddMinutes(5)
                };
                _dbContext.EmailSenderEntities.Add(EMAIL_ENTITY);
                await _dbContext.SaveChangesAsync();
                // END SAVE EMAIL ENTITY

                // START SEND EMAIL
                var mailMessage = new MailMessageDto(new List<string> { email }, TypeEmailConst.RECOVERY_PASSORD, otpCode);
                var result = await _emailAppService.SendEmailAsync(mailMessage);
                if (result.STATUS_CODE == 0) return new CommonReturnDto { STATUS_CODE = result.STATUS_CODE, ERROR_MESSAGE = "Send OTP successfully" };
                else return new CommonReturnDto { STATUS_CODE = result.STATUS_CODE, ERROR_MESSAGE = "Send OTP failure" };
                // END SEND EMAIL
            }
            catch (Exception ex)
            {
                throw new CustomException(-1, ex.Message);
            }
        }
        public async Task<CommonReturnDto> RecoveryPasswordAsync(RecoveryPasswordDto body)
        {
            var user = await _userManager.FindByEmailAsync(body.EMAIL);
            if (user == null) throw new CustomException(-1, "User not found");

            var emailSender = _dbContext.EmailSenderEntities
                .Where(e => e.RECEIVER_EMAIL == user.Email && e.TYPE_EMAIL == TypeEmailConst.RECOVERY_PASSORD 
                && e.CONTENT == body.OTP && e.IS_VALID && DateTime.Now < e.EXPIRE_DT).FirstOrDefault();

            if (emailSender == null) return new CommonReturnDto
            {
                STATUS_CODE = -1,
                ERROR_MESSAGE = "Invalid OTP or OTP had already expired"
            };
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, body.PASSWORD);
            emailSender.IS_VALID = false;
            _dbContext.EmailSenderEntities.Update(emailSender);
            await _dbContext.SaveChangesAsync();
            if (result.Succeeded) return new CommonReturnDto
            {
                STATUS_CODE = 0,
                ERROR_MESSAGE = "Recovery password successful"
            };
            else return new CommonReturnDto
            {
                STATUS_CODE = -1,
                ERROR_MESSAGE = "Recovery password failed"
            };
        }
    }
}
