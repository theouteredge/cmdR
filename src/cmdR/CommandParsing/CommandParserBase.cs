using cmdR.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmdR.CommandParsing
{
    public class CommandParserBase
    {
        protected string GetEscappedToken(string command, char terminator, char group, string escape, int position, out int nextposition)
        {
            var token = "";
            if (position >= command.Length)
            {
                nextposition = command.Length;
                return null;
            }

            while (command[position] == terminator)
            {
                position += 1;
            }

            if (command[position] == group)
            {
                var end = -1;
                var index = position + 1;
                var endOfToken = false;

                do
                {
                    end = command.IndexOf(group, index);
                    if (end == -1)
                    {
                        // start with an escape char, but we do not have a corresponding closing char, this command is invalid
                        throw new InvalidCommandException(string.Format("A command parameter starts with an group character of {0} at position {1} but does not have a terminating group character", group, position));
                    }

                    var groupEndPre = end - 1 < 0 ? "" : command.Substring(end - 1, 2);
                    var groupEndPost = end + 1 >= command.Length ? "" : command.Substring(end, 2);

                    // checking to see if the group char is being escapped or not
                    if (groupEndPre != escape || groupEndPost != escape)
                        endOfToken = true;
                }
                while (!endOfToken);

                token = command.Substring(index, end - index);
                nextposition = end + 2;
            }
            else token = GetUnescappedToken(command, terminator, position, out nextposition);

            return token;
        }


        /// <summary>
        /// gets the next unescaped token from the command, the first token in the command followed by a space, or if we dont have a space then we return the whole command as the token.
        /// </summary>
        protected string GetUnescappedToken(string command, char terminator, int position, out int nextposition)
        {
            var token = "";
            var nextTerminator = command.IndexOf(terminator, position);

            if (nextTerminator == -1)
            {
                token = command.Substring(position);
                nextposition = command.Length;
            }
            else
            {
                token = command.Substring(position, nextTerminator - position);
                nextposition = nextTerminator + 1;
            }

            return token;
        }
    }
}
