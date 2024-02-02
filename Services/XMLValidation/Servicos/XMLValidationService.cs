using System.Xml;
using System.Xml.Schema;
using NFeXMLValidator.Services.ServiceInterfaces;

namespace NFeXMLValidator.Services.Servicos
{
    public class XMLValidationService : IXMLValidationService
    {
        private static readonly ICollection<string> falhas = new List<string>();
        public string XMLValidate(string XML)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                document.LoadXml(XML);
                string falhas = ValidarXML(document);

                if (falhas.Count() > 0)
                    return falhas;
                else
                    return "Arquivo validado com sucesso!";
            }
            catch (Exception ex)
            {
                throw new System.Exception("Houve um erro ao gerar um documento XML com os dados recebidos." + ex.Message);
            }
        }

        private string ValidarXML(XmlDocument dados)
        {
            string retorno = string.Empty;

            ICollection<string> XSDFiles = new List<string>();
            try
            {
                if(Environment.OSVersion.Platform == System.PlatformID.Unix)
                {
                    XSDFiles.Add(@"Services/XMLValidation/Schemas/procNFe_v4.00.xsd");
                    XSDFiles.Add(@"Services/XMLValidation/Schemas/nfe_v4.00.xsd");
                    XSDFiles.Add(@"Services/XMLValidation/Schemas/leiauteNFe_v4.00.xsd");
                    XSDFiles.Add(@"Services/XMLValidation/Schemas/tiposBasico_v4.00.xsd");
                    XSDFiles.Add(@"Services/XMLValidation/Schemas/xmldsig-core-schema_v1.01.xsd");
                }
                else
                {
                    XSDFiles.Add(@"Services\XMLValidation\Schemas\procNFe_v4.00.xsd");
                    XSDFiles.Add(@"Services\XMLValidation\Schemas\nfe_v4.00.xsd");
                    XSDFiles.Add(@"Services\XMLValidation\Schemas\leiauteNFe_v4.00.xsd");
                    XSDFiles.Add(@"Services\XMLValidation\Schemas\tiposBasico_v4.00.xsd");
                    XSDFiles.Add(@"Services\XMLValidation\Schemas\xmldsig-core-schema_v1.01.xsd");
                }
                
            }
            catch(Exception ex)
            {
                throw ex;
            }

            List<string> validacao = ValidarDocumentoXML(dados, XSDFiles).ToList();

            if (validacao.Count() > 0)
            {
                retorno = $"Validação do XML:";
                foreach(var item in validacao)
                {
                    retorno += item;
                }
            }

            return retorno;
        }

        private static ICollection<string> ValidarDocumentoXML(XmlDocument doc, ICollection<string> XSDFiles)
        {
            falhas.Clear();
            try
            {
                foreach(var item in XSDFiles)
                {
                    doc.Schemas.Add(null, item);
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Houve um erro ao incluir os arquivos XSD para validar o arquivo XML. {ex.Message}");
            }

            try
            {
                doc.Validate(ValidationCallback);
            }
            catch(XmlSchemaValidationException ex)
            {
                throw new Exception($"Houve um erro ao executar a validação do documento XML. {ex.Message}");
            }

            return falhas;
        }

        private static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if(args.Severity == XmlSeverityType.Warning)
            {
                falhas.Add(" Alerta: " + TraduzMensagensDeErro(args.Message) + " (Caminho: " + ObtemCaminho(args) + ")");
            }
            if(args.Severity == XmlSeverityType.Error)
            {
                falhas.Add(" Erro: " + TraduzMensagensDeErro(args.Message) + " (Caminho: " + ObtemCaminho(args) + ")");
            }
        }

        private static string TraduzMensagensDeErro(string mensagem)
        {
            mensagem = mensagem.Replace("The value of the 'Algorithm' attribute does not equal its fixed value.", "O valor do atributo 'Algorithm' não é igual ao seu valor fixo.");
            mensagem = mensagem.Replace("The '", "O elemento '");
            mensagem = mensagem.Replace("element is invalid", "é inválido");
            mensagem = mensagem.Replace("The value", "O valor");
            mensagem = mensagem.Replace("is invalid according to its datatype", "é inválido de acordo com o seu tipo de dados");
            mensagem = mensagem.Replace("The Pattern constraint failed.", "");
            mensagem = mensagem.Replace("The actual length is less than the MinLength value", "O comprimento real é menor que o valor MinLength");
            mensagem = mensagem.Replace(" in namespace 'http://www.w3.org/2000/09/xmldsig#'.", "");
            mensagem = mensagem.Replace(" in namespace 'http://www.portalfiscal.inf.br/nfe'.", "");
            mensagem = mensagem.Replace(" in namespace 'http://www.portalfiscal.inf.br/nfe'", "");
            mensagem = mensagem.Replace("http://www.portalfiscal.inf.br/nfe:", "");
            mensagem = mensagem.Replace("The element", "O elemento");
            mensagem = mensagem.Replace("has invalid child element", "tem um elemento filho inválido");
            mensagem = mensagem.Replace("List of possible elements expected:", "Lista de possíveis elementos esperados:");
            mensagem = mensagem.Replace("The Enumeration constraint failed.", "");
            mensagem = mensagem.Replace("http://www.w3.org/2000/09/xmldsig#:", "");
            mensagem = mensagem.Replace("http://www.w3.org/2001/XMLSchema:", "");
            mensagem = mensagem.Replace("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters.", "A entrada não é uma string Base-64 válida, pois contém um caractere não base 64, mais de dois caracteres de preenchimento ou um caractere ilegal entre os caracteres de preenchimento.");
            mensagem = mensagem.Replace("The required attribute", "O atributo obrigatório");
            mensagem = mensagem.Replace("is missing", "está ausente");
            mensagem = mensagem.Replace("has incomplete content", "tem conteúdo incompleto");
            mensagem = mensagem.Replace("as well as", "bem como");
            return mensagem;
        }

        private static string ObtemCaminho(ValidationEventArgs args)
        {
            // Captura a referência para a tag que causou o problema (falha de schema)
            XmlSchemaValidationException ex = (XmlSchemaValidationException)args.Exception;
            object sourceObject = ex.SourceObject;

            if (sourceObject.GetType() == typeof(XmlElement))
            {
                XmlElement tagProblema = (XmlElement)(sourceObject);
                return GetCaminhoTagXML(tagProblema.ParentNode) + "/" + tagProblema.Name;
            }
            else
            {
                return "";
            }
        }

        private static string GetCaminhoTagXML(XmlNode args)
        {
            var node = args.ParentNode;
            if (args.ParentNode == null)
            {
                return "";
            }
            else if (args.ParentNode.NodeType == XmlNodeType.Element)
            {
                return GetCaminhoTagXML(node) + @"/" + args.Name;
            }
            return "";
        }
    }
}
