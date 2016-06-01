﻿using System.IO;
using Antlr4.Runtime;
using RDFCommon;

namespace SparqlQuery.SparqlClasses
{
    public static class SparqlQueryParser
    {
        public static Query.SparqlQuery Parse(IStore store, string sparqlString)
        {
            ICharStream input = new AntlrInputStream(sparqlString);

            var lexer = new sparq11lTranslatorLexer(input);

            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);

            var sparqlParser = new sparq11lTranslatorParser(commonTokenStream) { q = new RdfQuery11Translator(store) };



            return sparqlParser.query().value;

        }

        public static Query.SparqlQuery Parse(IStore store, Stream sparql)
        {

            ICharStream input = new AntlrInputStream(sparql);

            var lexer = new sparq11lTranslatorLexer(input);

            var commonTokenStream = new CommonTokenStream(lexer);

            var sparqlParser = new sparq11lTranslatorParser(commonTokenStream) { q = new RdfQuery11Translator(store) };



            return sparqlParser.query().value;

        }


    }
}
