using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEffects.Effect_Hosing
{
    static class Parsing
    {
        public List<Effect> GetEffects(string input)
        {
        }

        public bool IsTargeter(string input)
        {
            return ParseParenBlock(input) == "true" ? true : false;
        }

        public string ParseParenBlock(string input)
        {

            int parenCount = 0; int lastIndex = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                { parenCount++; lastIndex = input[i]; }

                if (input[i] == ')') parenCount--;

                if (parenCount == 0)
                {
                    // Gets first deepest string.                                                          | | |
                    var turn = input.Substring(lastIndex + 1, i - (lastIndex + 1));      // Recursive call V V V
                    input = input.Remove(lastIndex, (i + 1) - lastIndex).Insert(lastIndex, ParseParenBlock(turn));
                    return turn;
                }
            }

            // If we reached here, there are no substrings.
            // Do something with parseRegularTargeters.
            return "true";
        }

        public string ParseRegularTargeters(string text, PlayerParamArgs e)
        {
            // a | b & c

            
            byte style = 0; // 0 = building name 1 = building arg 2 = building thingie
            var argBuilder = new StringBuilder(); // Build arguments efficiently
            var BuiltTargeters = new List<Targeter>(); // List the targeters.
            var builtValues = new List<bool>();
            var Functs = new List<Func<Targeter, Targeter, bool>>(); // Functions1
            var Functions = new List<Func<bool, bool, bool>>(); // Functions.
            Targeter buildTargeter = null;

            for (int i = 0; i < text.Length; i++)
            {
                switch (style)
                {
                    case 0:
                        if (text[i] == '(')
                        {
                            var currentName = argBuilder.ToString().Trim();
                            argBuilder.Clear(); style++;
                            buildTargeter = API.Targeters.FirstOrDefault(t => t.Name == currentName);
                        }
                        
                            // If this says true/false
                        else if (argBuilder.ToString() == "true")
                        {
                            builtValues.Add(true);
                            argBuilder.Clear(); style++;
                        }
                        else if (argBuilder.ToString() == "false")
                        {
                            builtValues.Add(false);
                            argBuilder.Clear(); style++;
                        }

                        else argBuilder.Append(text[i]);
                        break;

                    case 1:
                        if (text[i] == ')')
                        {
                            var currentArg = argBuilder.ToString();
                            argBuilder.Clear(); style++;

                            BuiltTargeters.Add(buildTargeter.WithParam(currentArg));
                            builtValues.Add(buildTargeter.Affects(e.Player, currentArg));
                        }
                        break;
                    case 2:
                        style = 0;
                        if (text[i] == '=')
                        {
                            // replace text with function to string.
                            Functions.Add(XNor);
                        }
                        else if (text[i] == '^')
                        {
                            Functions.Add(XOr);
                        }
                        else if (text[i] == '|')
                        {
                            Functions.Add(Or);
                        }
                        else if (text[i] == '&')
                        {
                            Functions.Add(And);
                        }
                        else if (text[i] != ' ')
                        {
                            // TODO Exception handling.
                            throw new ArgumentException(i + ":Invalid arguments for logic operation!");
                        }
                        else style = 2; // Nicer way of writing it.
                        return;
                }
            }
        }

        private static bool And(bool a, bool b)
        {
            return a && b;
        }
        private static bool Or(bool a, bool b)
        {
            return a || b;
        }
        private static bool XOr(bool a, bool b)
        {
            return a ^ b;
        }
        private static bool XNor(bool a, bool b)
        {
            return a == b; // Also, the derivative of sine is cosine.
        }
    }
}
