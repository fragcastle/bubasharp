using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuBaSharp.Core;
using Xunit;

namespace BuBa.Core.Tests
{
    public class ScannerTests
    {
        private Scanner _scanner;

        [Fact]
        public void Test_Scanner_ReportsAddTokenCorrectly() 
        {
            _scanner = new Scanner( "var x = 10 + 10;" );

            Assert.Equal( true, _scanner.Tokens.Contains( ScannerTokens.Add ) );
        }

        [Fact]
        public void Test_Scanner_ReportsVariableTokenCorrectly()
        {
            _scanner = new Scanner( "var x = 10 + 10;" );

            Assert.Equal( ScannerTokens.Variable, (ScannerTokens)_scanner.Tokens[ 0 ] );
        }

        [Fact]
        public void Test_Scanner_ReportsVariableNameCorrectly()
        {
            _scanner = new Scanner( "var x = 10 + 10;" );

            Assert.Equal( "x", (string)_scanner.Tokens[ 1 ] );
        }

        [Fact]
        public void Test_Scanner_VariableNameWithUnderscores_ReportsVariableNameCorrectly()
        {
            _scanner = new Scanner( "var test_variable_name = 10 + 10;" );

            Assert.Equal( "test_variable_name", (string)_scanner.Tokens[ 1 ] );
        }

        [Fact]
        public void Test_Scanner_ReportsEqualsCorrectly()
        {
            _scanner = new Scanner( "var x = 10 + 10;" );

            Assert.Equal( ScannerTokens.Equal, (ScannerTokens)_scanner.Tokens[ 2 ] );
        }

        [Fact]
        public void Test_Scanner_Spec_CannotHaveVariableNameSameAsKeyword()
        {
            var e = Assert.Throws<Exception>( () => _scanner = new Scanner( "var var = 10 + 10;" ) );

            Assert.Equal( "Invalid variable name: 'var'", e.Message );
        }
    }
}
