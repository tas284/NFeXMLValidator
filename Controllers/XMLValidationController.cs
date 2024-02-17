using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using NFeXMLValidator.Interfaces;

namespace NFeXMLValidator.Controllers
{
    [Description("Validação de XML")]
    public class XMLValidationController : Controller
    {
        private readonly IXMLValidator _validator;

        public XMLValidationController(IXMLValidator validator)
        {
            _validator = validator;
        }

        [HttpPost("api/validarxml/")]
        public string Validar([FromBody] string xmlDocument)
        {
            return _validator.ValidateNFe(xmlDocument);
        }
    }
}
