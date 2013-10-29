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
                    // Gets first deepest string.
                    var turn = input.Substring(lastIndex + 1, i - (lastIndex + 1));
                    input = input.Remove(lastIndex, (i + 1) - lastIndex).Insert(lastIndex, ParseParenBlock(turn));
                    return turn;
                }
            }

            // If we reached here, there are no substrings.
            return "true";
        }

        public void ParseRegularTargeters(string text)
        {
            // a | b & c
        }
    }
}
