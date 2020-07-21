﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Parser.GherkinBuilder;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowLangParser
    {
        private readonly GherkinDialectServices dialectServices;

        public SpecFlowLangParser(CultureInfo defaultLanguage)
        {
            this.dialectServices = new GherkinDialectServices(defaultLanguage);
        }

        public Feature Parse(TextReader featureFileReader, string sourceFilePath)
        {
            var fileContent = featureFileReader.ReadToEnd();

            var gherkinDialect = dialectServices.GetGherkinDialectFromFileContent(fileContent);
            var gherkinListener = new GherkinParserListener(sourceFilePath);

            GherkinScannerAdapter scannerAdapter = new GherkinScannerAdapter(gherkinDialect, fileContent);
            scannerAdapter.Scan(gherkinListener);

            Feature feature = gherkinListener.GetResult();

            if (gherkinListener.Errors.Count > 0)
                throw new SpecFlowParserException(gherkinListener.Errors);

            Debug.Assert(feature != null, "If there were no errors, the feature cannot be null");
            
            //feature.Language = language.LanguageForConversions.Name;
            feature.Language = gherkinDialect.CultureInfoForConversions.Name;

            return feature;
        }
    }
}
