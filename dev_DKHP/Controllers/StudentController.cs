using dev_DKHP.CoreModule.Dto;
using dev_DKHP.Intfs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
/*
{
"userName": "20521153",
"normalizedUserName": "20521153",
"email": "20521153@gm.uit.edu.vn",
"normalizedEmail": "20521153@gm.uit.edu.vn",
"emailConfirmed": true,
"phoneNumber": "0387383583",
"phoneNumberConfirmed": true,
"twoFactorEnabled": false,
"lockoutEnd": "2024-08-23T03:48:27.697Z",
"lockoutEnabled": true,
"accessFailedCount": 0,
"useR_CODE": "20521153",
"deP_CODE": "1234",
"password": "123456Aa.`",
  "roles": [
    {
      "userId": "string",
      "roleId": "STUDENT"
    },
    {
      "userId": "string",
      "roleId": "STUDENTSSSS"
    }
  ]
}

{
  "userNameOrEmailAddress": "20521153",
  "password": "123456Aa.`"
}


*/
namespace dev_DKHP.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentAppService _studentAppService;

        public StudentController(IStudentAppService studentAppService)
        {
            _studentAppService = studentAppService;
        }

        [HttpPost]
        public async Task<List<object>> TestApi()
        {
            return await _studentAppService.TestApi();
        }

        [HttpPost]
        public async Task<List<ENROLLED_STUDENT_ENTITY>> ENROLLED_STUDENT_SEARCH(ENROLLED_STUDENT_ENTITY filterInput)
        {
            return await _studentAppService.ENROLLED_STUDENT_SEARCH(filterInput);
        }

        [HttpPost]
        public async Task<IDictionary<string, object>> ENROLLED_STUDENT_INS(ENROLLED_STUDENT_ENTITY input)
        {
            return await _studentAppService.ENROLLED_STUDENT_INS(input);
        }

    }
}
