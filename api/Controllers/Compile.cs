using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using analyzer;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace api.Controllers
{
    [Route("[controller]")]
    public class Compile : Controller
    {
        private readonly ILogger<Compile> _logger;

        public Compile(ILogger<Compile> logger)
        {
            _logger = logger;
        }

        public class CompileRequest
        {
            [Required]
            public required string code { get; set; }
        }

        // POST /compile
        [HttpPost]
        public IActionResult Post([FromBody] CompileRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Invalid request" });
            }

            var inputStream = new AntlrInputStream(request.code);
            var lexer = new LanguageLexer(inputStream);

            //Vamos a quitar los listeners default de ANTLR y crear uno propio
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new LexicalErrorListener());

            var tokens = new CommonTokenStream(lexer);
            var parser = new LanguageParser(tokens);

            //Hacemos lo mismo con el parser 
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new SyntaxErrorListener());

            try{
                var tree = parser.program();
                var visitor = new CompilerVisitor();
                visitor.Visit(tree);

            return Ok(new { result = visitor.output });
            }
            catch(ParseCanceledException ex){
                return BadRequest (new {error= ex.Message});
            }
            catch(SemanticError ex){
                return BadRequest(new {error = ex.Message});
            }

            catch(ContinueException ex){
                return BadRequest(new {error = "Haz usado un continue fuera de un ciclo"});
            }
            catch(BreakException ex){
                return BadRequest(new {error = "Haz usado un break fuera de un ciclo"});
            }
        }

    }
}
