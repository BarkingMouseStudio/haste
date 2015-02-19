using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Haste {

  public static class JSON {

    sealed class Parser : IDisposable {
      const string WHITE_SPACE = " \t\n\r";
      const string WORD_BREAK = " \t\n\r{}[],:\"";

      enum Token {
        NONE,
        CURLY_OPEN,
        CURLY_CLOSE,
        SQUARED_OPEN,
        SQUARED_CLOSE,
        COLON,
        COMMA,
        STRING,
        NUMBER,
        TRUE,
        FALSE,
        NULL
      };

      StringReader json;

      Parser(string jsonString) {
        json = new StringReader(jsonString);
      }

      public static object Parse(string jsonString) {
        using (var instance = new Parser(jsonString)) {
          return instance.ParseValue();
        }
      }

      public void Dispose() {
        json.Dispose();
        json = null;
      }

      Dictionary<string, object> ParseObject() {
        Dictionary<string, object> table = new Dictionary<string, object>();

        // ditch opening brace
        json.Read();

        // {
        while (true) {
          switch (NextToken) {
            case Token.NONE:
              return null;
            case Token.COMMA:
              continue;
            case Token.CURLY_CLOSE:
              return table;
            default:
              // name
              string name = ParseString();
              if (name == null) {
                return null;
              }

              // :
              if (NextToken != Token.COLON) {
                return null;
              }
              // ditch the colon
              json.Read();

              // value
              table[name] = ParseValue();
              break;
          }
        }
      }

      List<object> ParseArray() {
        List<object> array = new List<object>();

        // ditch opening bracket
        json.Read();

        // [
        var parsing = true;
        while (parsing) {
          Token nextToken = NextToken;

          switch (nextToken) {
            case Token.NONE:
              return null;
            case Token.COMMA:
              continue;
            case Token.SQUARED_CLOSE:
              parsing = false;
              break;
            default:
              object value = ParseByToken(nextToken);
              array.Add(value);
              break;
          }
        }

        return array;
      }

      object ParseValue() {
        Token nextToken = NextToken;
        return ParseByToken(nextToken);
      }

      object ParseByToken(Token token) {
        switch (token) {
          case Token.STRING:
            return ParseString();
          case Token.NUMBER:
            return ParseNumber();
          case Token.CURLY_OPEN:
            return ParseObject();
          case Token.SQUARED_OPEN:
            return ParseArray();
          case Token.TRUE:
            return true;
          case Token.FALSE:
            return false;
          case Token.NULL:
            return null;
          default:
            return null;
        }
      }

      string ParseString() {
        StringBuilder s = new StringBuilder();
        char c;

        // ditch opening quote
        json.Read();

        bool parsing = true;
        while (parsing) {

          if (json.Peek() == -1) {
            parsing = false;
            break;
          }

          c = NextChar;
          switch (c) {
            case '"':
              parsing = false;
              break;
            case '\\':
              if (json.Peek() == -1) {
                parsing = false;
                break;
              }

              c = NextChar;
              switch (c) {
              case '"':
              case '\\':
              case '/':
                s.Append(c);
                break;
              case 'b':
                s.Append('\b');
                break;
              case 'f':
                s.Append('\f');
                break;
              case 'n':
                s.Append('\n');
                break;
              case 'r':
                s.Append('\r');
                break;
              case 't':
                s.Append('\t');
                break;
              case 'u':
                var hex = new StringBuilder();

                for (int i=0; i< 4; i++) {
                  hex.Append(NextChar);
                }

                s.Append((char) Convert.ToInt32(hex.ToString(), 16));
                break;
              }
              break;
            default:
              s.Append(c);
              break;
          }
        }

        return s.ToString();
      }

      object ParseNumber() {
        string number = NextWord;

        if (number.IndexOf('.') == -1) {
          long parsedInt;
          Int64.TryParse(number, out parsedInt);
          return parsedInt;
        }

        double parsedDouble;
        Double.TryParse(number, out parsedDouble);
        return parsedDouble;
      }

      void EatWhitespace() {
        while (WHITE_SPACE.IndexOf(PeekChar) != -1) {
          json.Read();

          if (json.Peek() == -1) {
            break;
          }
        }
      }

      char PeekChar {
        get {
          return Convert.ToChar(json.Peek());
        }
      }

      char NextChar {
        get {
          return Convert.ToChar(json.Read());
        }
      }

      string NextWord {
        get {
          StringBuilder word = new StringBuilder();

          while (WORD_BREAK.IndexOf(PeekChar) == -1) {
            word.Append(NextChar);

            if (json.Peek() == -1) {
              break;
            }
          }

          return word.ToString();
        }
      }

      Token NextToken {
        get {
          EatWhitespace();

          if (json.Peek() == -1) {
            return Token.NONE;
          }

          char c = PeekChar;
          switch (c) {
          case '{':
            return Token.CURLY_OPEN;
          case '}':
            json.Read();
            return Token.CURLY_CLOSE;
          case '[':
            return Token.SQUARED_OPEN;
          case ']':
            json.Read();
            return Token.SQUARED_CLOSE;
          case ',':
            json.Read();
            return Token.COMMA;
          case '"':
            return Token.STRING;
          case ':':
            return Token.COLON;
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
          case '-':
            return Token.NUMBER;
          }

          string word = NextWord;

          switch (word) {
          case "false":
            return Token.FALSE;
          case "true":
            return Token.TRUE;
          case "null":
            return Token.NULL;
          }

          return Token.NONE;
        }
      }
    }

    sealed class Serializer {
      StringBuilder builder;

      Serializer() {
        builder = new StringBuilder();
      }

      public static string Serialize(object obj) {
        var instance = new Serializer();
        instance.SerializeValue(obj);
        return instance.builder.ToString();
      }

      void SerializeValue(object value) {
        IList asList;
        IDictionary asDict;
        string asStr;

        if (value == null) {
          builder.Append("null");
        } else if ((asStr = value as string) != null) {
          SerializeString(asStr);
        } else if (value is bool) {
          builder.Append(value.ToString().ToLower());
        } else if ((asList = value as IList) != null) {
          SerializeArray(asList);
        } else if ((asDict = value as IDictionary) != null) {
          SerializeObject(asDict);
        } else if (value is char) {
          SerializeString(value.ToString());
        } else {
          SerializeOther(value);
        }
      }

      void SerializeObject(IDictionary obj) {
        bool first = true;

        builder.Append('{');

        foreach (object e in obj.Keys) {
          if (!first) {
            builder.Append(',');
          }

          SerializeString(e.ToString());
          builder.Append(':');

          SerializeValue(obj[e]);

          first = false;
        }

        builder.Append('}');
      }

      void SerializeArray(IList anArray) {
        builder.Append('[');

        bool first = true;

        foreach (object obj in anArray) {
          if (!first) {
            builder.Append(',');
          }

          SerializeValue(obj);

          first = false;
        }

        builder.Append(']');
      }

      void SerializeString(string str) {
        builder.Append('\"');

        char[] charArray = str.ToCharArray();
        foreach (var c in charArray) {
          switch (c) {
            case '"':
              builder.Append("\\\"");
              break;
            case '\\':
              builder.Append("\\\\");
              break;
            case '\b':
              builder.Append("\\b");
              break;
            case '\f':
              builder.Append("\\f");
              break;
            case '\n':
              builder.Append("\\n");
              break;
            case '\r':
              builder.Append("\\r");
              break;
            case '\t':
              builder.Append("\\t");
              break;
            default:
              int codepoint = Convert.ToInt32(c);
              if ((codepoint >= 32) && (codepoint <= 126)) {
                builder.Append(c);
              }
              else {
                builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
              }
              break;
          }
        }

        builder.Append('\"');
      }

      void SerializeOther(object value) {
          if (value is float
              || value is int
              || value is uint
              || value is long
              || value is double
              || value is sbyte
              || value is byte
              || value is short
              || value is ushort
              || value is ulong
              || value is decimal) {
              builder.Append(value.ToString());
          } else {
            SerializeString(value.ToString());
          }
        }
    }

    public static object Deserialize(string json) {
      if (json == null) {
        return null;
      }

      return Parser.Parse(json);
    }

    public static string Serialize(object obj) {
      return Serializer.Serialize(obj);
    }
  }
}
