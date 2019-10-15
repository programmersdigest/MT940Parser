using programmersdigest.MT940Parser.Parsing;
using System;
using System.Collections.Generic;
using System.IO;

namespace programmersdigest.MT940Parser
{
    public class Parser
    {
        private string _path = null;
        private Stream _stream = null;

        public Parser(string path)
        {
            _path = path;
        }

        public Parser(Stream stream)
        {
            _stream = stream;
        }

        public IEnumerable<Statement> Parse()
        {
            using StreamReader _reader = (_stream == null) ? new StreamReader(_path) : new StreamReader(_stream);
            while (!_reader.EndOfStream)
            {
                StatementParser _statementParser = new StatementParser(_reader);

                var statement = _statementParser.ReadStatement();

                if (statement != null)
                {
                    yield return statement;
                }
            }
        }

    }
}
