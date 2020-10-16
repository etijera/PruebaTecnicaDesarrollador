using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tienda.Controllers;

namespace TiendaTest.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        // Clase Base de Datos Fake
        //private SqLiteDbFake sqLiteDbFake;

        [TestInitialize]
        public void Init()
        {
            //sqLiteDbFake = new SqLiteDbFake();
        }

        [TestMethod]
        public void IndexElViewResultNoEsNull()
        {
            // Arange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
