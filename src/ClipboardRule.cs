﻿namespace ClipboardManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class ClipboardRule
    {
        public static IEnumerable<ClipboardRule> GetMatchingRules(string text)
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                foreach (var rule in ClipboardRules)
                {
                    if (rule.IsMatch(text, out string[] values))
                    {
                        yield return new ClipboardRule
                        {
                            Label = String.Format(rule.Label, values),
                            Values = values,
                            QuickActions = rule.QuickActions,
                        };
                    }
                }
            }
        }

        internal static List<ClipboardRule> ClipboardRules = new List<ClipboardRule>()
        {
            new ClipboardRule
            {
                Label = "Redmine issue",
                /*language=regex*/
                RegexPattern = @"
                    ^
                    \s*
                    (?i:
                        refs\s?\#?
                        |
                        RM\s?
                        |
                        (?:\#)
                    )
                    ([0-9]+)
                    \s*
                    $
                    ",
                QuickActions = new List<QuickAction>()
                {
                    new QuickAction
                    {
                        CanCopy = true,
                        Name = "Redmine",
                        OpenLabel = "Open Redmine issue",
                        Url = "https://redmine.neovici.se/issues/{1}",
                    }
                }
            },
            new ClipboardRule
            {
                Label = "DevOps PR",
                /*language=regex*/
                RegexPattern = @"
                    (?i)
                    ^
                    \s*
                    PR\s?
                    ([0-9]+)
                    \s*
                    $
                    ",
                QuickActions = new List<QuickAction>()
                {
                    new QuickAction
                    {
                        CanCopy = true,
                        Name = "Pull Request",
                        OpenLabel = "View Pull Request",
                        Url = "https://neovici.visualstudio.com/Cosmoz3/Cosmoz3%20Team/_git/cz3backend/pullrequest/{1}",
                    }
                }
            },
            new ClipboardRule
            {
                Label = "Guid",
                /*language=regex*/
                RegexPattern = @"
                    (?im)                       
                    ^
                    \s*
                    [{(]?
                    [0-9A-F]{8}
                    [-]?
                    (?:[0-9A-F]{4}[-]?){3}
                    [0-9A-F]{12}
                    [)}]? 
                    \s*
                    $",
                QuickActions = new List<QuickAction>()
                {
                    new QuickAction
                    {
                        CanCopy = true,
                        Name = "Guid",
                        OpenLabel = "Copy guid",
                    }
                }
            },
        };

        public ClipboardRule()
        {
            QuickActions = new List<QuickAction>();
            Label = "{0}";
        }

        public string Label { get; set; }

        public string[] Values { get; set; }

        public Regex RuleRegex { get; private set; }

        public string RegexPattern
        {
            set
            {
                RuleRegex = new Regex(value, RegexOptions.IgnorePatternWhitespace);
            }
        }

        public IList<QuickAction> QuickActions { get; set; }

        public virtual bool IsMatch(string input, out string[] output)
        {
            var match = RuleRegex.Match(input);

            if (match.Success)
            {
                output = match.Groups.OfType<Group>().Select(g => g.Value).ToArray();

                return true;
            }

            output = null;

            return false;
        }
    }
}
