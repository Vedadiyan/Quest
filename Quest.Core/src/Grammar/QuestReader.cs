using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
        public List<object> QuestTokens { get; }
        public QuestReader(string questQuery)
        {
            this.questQuery = questQuery.TrimStart().TrimEnd();
            this.len = questQuery.Length;
            QuestTokens = parse();
        }
        public bool Eval<T>(T value) {
            return eval(value);
        }
        private List<object> parse()
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
                            questTokens.Add(parse());
                        }
                        else
                        {
                            questToken.Value = parse();
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
            return questTokens;
        }
        private string readString()
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
        private bool eval<T>(T value, List<object> _QuestTokens = null, bool isAnd = false)
        {
            bool result = true;
            foreach (var token in _QuestTokens ?? QuestTokens)
            {
                if (token is QuestToken questToken)
                {
                    switch (questToken.Key)
                    {
                        case Tokens.AND_ALSO:
                            {
                                result &= eval<T>(value, (List<object>)questToken.Value, true);
                                break;
                            }
                        case Tokens.OR_ELSE:
                            {
                                result = false;
                                result |= eval<T>(value, (List<object>)questToken.Value);
                                break;
                            }
                        default:
                            {
                                if (isAnd)
                                {
                                    result &= eval<T>(value, questToken);
                                }
                                else
                                {
                                    result = false;
                                    result |= eval<T>(value, questToken);
                                    if (result)
                                    {
                                        return result;
                                    }
                                }
                                break;
                            }
                    }
                }
                else if (token is List<object> tokens)
                {
                    if (isAnd)
                    {
                        result &= eval<T>(value, tokens, isAnd);
                    }
                    else
                    {
                        result = false;
                        result |= eval<T>(value, tokens, isAnd);
                        if (result)
                        {
                            return result;
                        }
                    }
                }
            }
            return result;
        }
        private bool eval<T>(T value, QuestToken questToken)
        {
            bool result = true;
            switch (questToken.Key)
            {
                case Tokens.IN:
                    {
                        result = false;
                        foreach (var innerExp in (List<object>)questToken.Value)
                        {
                            if (innerExp is QuestToken _questToken)
                            {
                                result |= eval<T>(value, _questToken);
                            }
                            else
                            {
                                if (innerExp is string)
                                {
                                    result |= value.ToString().Equals(innerExp);
                                }
                                else
                                {
                                    if (value is decimal d)
                                    {
                                        result |= (decimal)innerExp == d;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case Tokens.NOT_IN:
                    {
                        result = true;
                        foreach (var innerExp in (List<object>)questToken.Value)
                        {
                            if (innerExp is QuestToken _questToken)
                            {
                                result &= eval<T>(value, _questToken);
                            }
                            else
                            {
                                if (innerExp is string)
                                {
                                    result &= !value.ToString().Equals(innerExp);
                                }
                                else
                                {
                                    if (value is decimal d)
                                    {
                                        result &= (decimal)innerExp != d;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case Tokens.GREATER_THAN:
                    {
                        if (value is decimal d)
                        {
                            result = ((decimal)questToken.Value > d);
                        }
                        break;
                    }
                case Tokens.GREATER_THAN_EQUAL:
                    {
                        if (value is decimal d)
                        {
                            result = ((decimal)questToken.Value >= d);
                        }
                        break;
                    }
                case Tokens.LOWER_THAN_EQUAL:
                    {
                        if (value is decimal d)
                        {
                            result = ((decimal)questToken.Value <= d);
                        }
                        break;
                    }
                case Tokens.NOT_EQUAL:
                    {
                        if (value is decimal d)
                        {
                            result = ((decimal)questToken.Value != d);
                        }
                        else if (value is string str)
                        {
                            result = !questToken.Value.ToString().Equals(str);
                        }
                        break;
                    }
                case Tokens.EQUAL:
                    {
                        if (value is decimal d)
                        {
                            result = ((decimal)questToken.Value != d);
                        }
                        else if (value is string str)
                        {
                            result = questToken.Value.ToString().Equals(str);
                        }
                        break;
                    }
                case Tokens.PATTERN:
                    {
                        if (value is string str)
                        {
                            Regex regex = new Regex(questToken.Value.ToString());
                            result = regex.IsMatch(value.ToString());
                        }
                        break;
                    }

            }
            return result;
        }
    }
}