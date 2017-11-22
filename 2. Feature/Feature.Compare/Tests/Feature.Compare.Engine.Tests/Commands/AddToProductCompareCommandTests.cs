using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SxaS.Compare.Engine.Commands;

namespace Feature.Compare.Engine.Tests.Commands
{
    [TestClass]
    public class AddToProductCompareCommandTests
    {
        [TestMethod]
        public void CanCreateInstanceofCommand()
        {
            //arrange

            //act
            var command = new AddToProductCompareCommand(null, null, null);

            //assert
            Assert.IsNotNull(command);
        }
    }
}
