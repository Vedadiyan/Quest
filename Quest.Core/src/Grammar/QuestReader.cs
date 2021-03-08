using System;
using System.Collections.Generic;
using System.Text;

namespace Quest.Core.Grammar
{
    public class QuestToken
    {
        private string key;
        public object Key
        {
            get
            {
                if (key == null)
                {
                    return null;
                }
                switch (key)
                {
                    case "and":
                        return Tokens.AND_ALSO;
                    case "or":
                        return Tokens.OR_ELSE;
                    case "lt":
                        return Tokens.LOWER_THAN;
                    case "lte":
                        return Tokens.LOWER_THAN_EQUAL;
                    case "gt":
                        return Tokens.GREATER_THAN;
                    case "gte":
                        return Tokens.GREATER_THAN_EQUAL;
                    case "eq":
                        return Tokens.EQUAL;
                    case "not":
                        return Tokens.NOT_EQUAL;
                    case "in":
                        return Tokens.IN;
                    case "matches":
                        return Tokens.PATTERN;
                }
                throw new Exception("Invalid Quest Query Operator");
            }
            set { key = value.ToString(); }
        }
        public object Value { get; set; }
    }
    public class QuestReader
    {
        private readonly string questQuery;
        private int index = 0;
        private int len;
        public QuestReader(string questQuery)
        {
            this.questQuery = questQuery.TrimStart().TrimEnd();
            this.len = questQuery.Length;
        }
        public List<object> Parse()
        {
            if (!questQuery.StartsWith('{') || !questQuery.EndsWith('}'))
            {
                throw new System.Exception("Invalid Quest Query");
            }
            StringBuilder buffer = new StringBuilder();
            QuestToken questToken = new();
            index++;
            List<object> questTokens = new List<object>();
            char _ref = '\0';
            for (; index < len; index++)
            {
                _ref = questQuery[index];
                switch (_ref)
                {
                    case ':':
                        questToken.Key = buffer.ToString();
                        buffer.Clear();
                        break;
                    case ',':
                        if (questToken.Key == null && buffer.Length > 0)
                        {
                            questTokens.Add(Convert.ToDecimal(buffer.ToString()));
                        }
                        else if(buffer.Length > 0)
                        {
                            if (char.IsDigit(buffer[0]))
                            {
                                questToken.Value = Convert.ToDecimal(buffer.ToString());
                            }
                            else
                            {
                                switch (buffer.ToString().ToLower())
                                {
                                    case "null":
                                        questToken.Value = null;
                                        break;
                                    case "true":
                                        questToken.Value = true;
                                        break;
                                    case "false":
                                        questToken.Value = false;
                                        break;
                                }
                            }
                            questTokens.Add(questToken);
                        }
                        buffer.Clear();
                        questToken = new();
                        break;
                    case '\'':
                        if (questToken.Key == null)
                        {
                            questTokens.Add(readString());
                        }
                        else
                        {
                            questToken.Value = readString();
                            buffer.Clear();
                            questTokens.Add(questToken);
                        }
                        questToken = new();
                        break;
                    case '[':
                    case '{':
                        if (questToken.Key == null)
                        {
                            questTokens.Add(Parse());
                        }
                        else
                        {
                            questToken.Value = Parse();
                            buffer.Clear();
                            questTokens.Add(questToken);
                        }
                        questToken = new();
                        break;
                    case ']':
                    case '}':
                        if (questToken.Key == null && buffer.Length > 0)
                        {
                            questTokens.Add(Convert.ToDecimal(buffer.ToString()));
                        }
                        else if (buffer.Length > 0)
                        {
                            if (char.IsDigit(buffer[0]))
                            {
                                questToken.Value = Convert.ToDecimal(buffer.ToString());
                            }
                            else
                            {
                                switch (buffer.ToString().ToLower())
                                {
                                    case "null":
                                        questToken.Value = null;
                                        break;
                                    case "true":
                                        questToken.Value = true;
                                        break;
                                    case "false":
                                        questToken.Value = false;
                                        break;
                                }
                            }
                            questTokens.Add(questToken);
                        }
                        buffer.Clear();
                        questToken = new();
                        return questTokens;
                    default:
                        if (!(buffer.Length == 0 && char.IsWhiteSpace(_ref)))
                        {
                            buffer.Append(_ref);
                        }
                        break;
                }
            }
            throw new System.Exception("Badly Formatted Quest Query");
        }
        public string readString()
        {
            StringBuilder buffer = new StringBuilder();
            bool skipNext = false;
            index++;
            for (; index < len; index++)
            {
                char _ref = questQuery[index];
                switch (_ref)
                {
                    case '\\':
                        skipNext = true;
                        break;
                    case '\'':
                        if (skipNext)
                        {
                            skipNext = false;
                            buffer.Append(_ref);
                        }
                        else
                        {
                            index++;
                            return buffer.ToString();
                        }
                        break;
                    default:
                        skipNext = false;
                        buffer.Append(_ref);
                        break;
                }
            }
            throw new System.Exception("Badly Formatted String Value");
        }
    }
}