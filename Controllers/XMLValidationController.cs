using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using NFeXMLValidator.Services.ServiceInterfaces;

namespace NFeXMLValidator.Controllers
{
    [Description("Validação de XML")]
    public class XMLValidationController : Controller
    {
        private readonly IXMLValidationService _XMLValidationService;

        public XMLValidationController(IXMLValidationService XMLValidationService)
        {
            _XMLValidationService = XMLValidationService;
        }

        [HttpPost("api/validarxml/")]
        public string Validar([FromBody] string xmlDocument)
        {
            return _XMLValidationService.XMLValidate(xmlDocument);
        }
    }
}
