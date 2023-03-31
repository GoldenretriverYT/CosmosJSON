using System.Data.SqlTypes;
using System.Globalization;
using System.Security;
using System.Text.Json.Nodes;

namespace CosmosJSON
{
    public class Parser
    {
        private string code;
        private int pos;

        public char Current => Peek(0);

        public char Peek(int off)
        {
            if (pos + off >= code.Length || pos + off < 0) return '\0';
            return code[pos + off];
        }

        public char Next()
        {
            pos++;
            return Current;
        }

        public void MatchChar(char c)
        {
            if(Current == c) {
                Next();
                return;
            }

            throw new Exception($"Expected character '{c}' at offset {pos}");
        }

        public bool MatchSeq(string seq)
        {
            for(var i = 0; i < seq.Length; i++) {
                if(Current == seq[i])
                    Next();
                else
                    throw new Exception($"Expected character '{seq[i]}' at offset {pos}");
            }

            return true;
        }

        public Parser(string code)
        {
            this.code = code;
        }

        public Dictionary<string, object> Parse()
        {
            return ParseObject();
        }

        public Dictionary<string, object> ParseObject()
        {
            var obj = new Dictionary<string, object>();

            MatchChar('{');
            ParseWhitespace();

            do {
                if (Current == ',') MatchChar(',');

                var key = ParseString();
                ParseWhitespace();
                MatchChar(':');
                var val = ParseValue();

                obj.Add(key, val);
            } while (Current == ',');

            ParseWhitespace();
            MatchChar('}');

            return obj;
        }

        public object ParseValue()
        {
            ParseWhitespace();

            if (Current == '"')
                return ParseString();
            else if (char.IsDigit(Current))
                return ParseNumber();
            else if (Current == '{')
                return ParseObject();
            else if (Current == '[')
                return ParseArray();
            else if (Current == 't')
                return MatchSeq("true");
            else if (Current == 'f')
                return !MatchSeq("false");
            else if (Current == 'n')
                return MatchSeq("null") ? null : null;
            else
                throw new Exception($"Unexpected character {Current} at position {pos}");
        }

        public object[] ParseArray()
        {
            var list = new List<object>();
            MatchChar('[');

            ParseWhitespace();
            do {
                if (Current == ',') MatchChar(',');

                ParseWhitespace();
                var val = ParseValue();
                ParseWhitespace();

                list.Add(val);
            } while (Current == ',');

            ParseWhitespace();
            MatchChar(']');
            return list.ToArray();
        }

        public string ParseString()
        {
            ParseWhitespace();
            MatchChar('"');

            string r = "";

            while(true) {
                if (Current == '\0') throw new Exception($"Unexpected end of file in string sequence at position {pos}");

                if(Current == '\\') {
                    var esc = Next();

                    switch(esc) {
                        case '"': r += "\""; break;
                        case '\\': r += "\\"; break;
                        case '/': r += "/"; break;
                        case 'b': r += "\b"; break;
                        case 'f': r += "\f"; break;
                        case 'n': r += "\n"; break;
                        case 'r': r += "\r"; break;
                        case 't': r += "\t"; break;
                        case 'u': throw new NotImplementedException("Unimplemented: Parse unicode codepoint escape sequence");
                        default: throw new Exception($"Unexpected escape sequence '\\{esc}' at position {pos}");
                    }

                    Next();
                    continue;
                }

                if (Current == '"') {
                    MatchChar('"');
                    break;
                }

                r += Current;
                Next();
            }

            return r;
        }

        public void ParseWhitespace()
        {
            while (char.IsWhiteSpace(Current)) Next();
        }

        public object ParseNumber()
        {
            var str = "";
            var hasFract = false;

            while(Current != '\0') {
                if(char.IsDigit(Current) || Current is 'e' or '+' or '-' or '.') {
                    if (Current == '.') {
                        if (hasFract) throw new Exception($"Number has 2 fraction symbols at position {pos}");
                        hasFract = true;
                    }

                    str += Current;
                    Next();
                    continue;
                }

                break;
            }

            if (hasFract) {
                if (!float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out float resF))
                    throw new Exception($"Float parsing failed at position {pos}");
                return resF;
            } else {
                if (!int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out int resI))
                    throw new Exception($"Integer parsing failed at position {pos}");
                return resI;
            }
        }
    }
}