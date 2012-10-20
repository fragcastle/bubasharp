using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuBaSharp.Core
{
    // Constants to represent arithmitic tokens. This could
    // be alternatively written as an enum.
    public enum ScannerTokens
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Semi,
        Equal,
        Variable
    }

    public abstract class CompilationErrors
    {
        public static Exception UnterminatedStringLiteral = new Exception( "Unterminated String Literal." );

        public static Exception InvalidVariableName( string name )
        {
            return new Exception( "Invalid variable name: '" + name + "'" );
        }

        public static Exception UnrecognizedCharacter( char character )
        {
            return new Exception( "Scanner encountered unrecognized character '" + character + "'" );
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Language spec
    /// +Something( String a, Object b, Variant c) {
    ///     10 + 1;
    /// }
    /// </remarks>
    public sealed class Scanner
    {
        private readonly IList<object> _result = new List<object>();

        public Scanner( String input )
        {
            using( var reader = new StringReader( input ) )
            {
                Scan( reader );
            }
        }

        public Scanner( TextReader input )
        {
            Scan( input );
        }

        public IList<object> Tokens
        {
            get { return _result; }
        }

        private bool IsKeyword( string word )
        {
            return new string[] {
                "var", "+", "-", "/", "*", ";", "\""
            }.Contains( word );
        }

        private void Scan( TextReader input )
        {
            while( input.Peek() != -1 )
            {
                var accum = new StringBuilder();
                char ch = (char)input.Peek();

                // Scan individual tokens
                if( char.IsWhiteSpace( ch ) )
                {
                    // eat the current char and skip ahead!
                    input.Read();
                }
                else if( char.IsLetter( ch ) || ch == '_' )
                {
                    // keyword or identifier
                    while( char.IsLetter( ch ) || ch == '_' )
                    {
                        accum.Append( ch );
                        input.Read();

                        if( input.Peek() == -1 )
                        {
                            break;
                        }
                        else
                        {
                            ch = (char)input.Peek();
                        }
                    }

                    ScannerTokens? lastToken = null;

                    if ( _result.Count > 0 )
                        lastToken = _result.Last() as ScannerTokens?;

                    if( accum.ToString() == "var" && lastToken == null )
                    {
                        _result.Add( ScannerTokens.Variable );
                    }
                    else
                    {
                        // check if the previous token was a variable token,
                        // if so, we must have a valid variable name to continue
                        // last token was a variable... validate the variable name
                        var validVariableName = new System.Text.RegularExpressions.Regex( "\\w+" );
                        var isValid = validVariableName.IsMatch( accum.ToString() );
                        if( IsKeyword( accum.ToString() ) || !isValid )
                        {
                            throw CompilationErrors.InvalidVariableName( accum.ToString() );
                        }
                        
                        // last token wasn't a variable decl so continue on
                        _result.Add( accum.ToString() );
                    }
                }
                else if( ch == '"' )
                {
                    input.Read(); // skip the '"'

                    if( input.Peek() == -1 )
                    {
                        throw CompilationErrors.UnterminatedStringLiteral;
                    }

                    while( ( ch = (char)input.Peek() ) != '"' )
                    {
                        accum.Append( ch );
                        input.Read();

                        if( input.Peek() == -1 )
                        {
                            throw CompilationErrors.UnterminatedStringLiteral;
                        }
                    }

                    // skip the terminating "
                    input.Read();
                    _result.Add( accum );
                }
                else if( char.IsDigit( ch ) )
                {
                    while( char.IsDigit( ch ) )
                    {
                        accum.Append( ch );
                        input.Read();

                        if( input.Peek() == -1 )
                        {
                            break;
                        }
                        else
                        {
                            ch = (char)input.Peek();
                        }
                    }

                    _result.Add( int.Parse( accum.ToString() ) );
                }
                else switch( ch )
                    {
                        case '+':
                            input.Read();
                            _result.Add( ScannerTokens.Add );
                            break;

                        case '-':
                            input.Read();
                            _result.Add( ScannerTokens.Subtract );
                            break;

                        case '*':
                            input.Read();
                            _result.Add( ScannerTokens.Multiply );
                            break;

                        case '/':
                            input.Read();
                            _result.Add( ScannerTokens.Divide );
                            break;

                        case '=':
                            input.Read();
                            _result.Add( ScannerTokens.Equal );
                            break;

                        case ';':
                            input.Read();
                            _result.Add( ScannerTokens.Semi );
                            break;

                        default:
                            throw CompilationErrors.UnrecognizedCharacter( ch );
                    }

            }
        }
    }
}
