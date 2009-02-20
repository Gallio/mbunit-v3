Imports System
Imports System.Text
Imports System.Collections.Generic
Imports System.Web.Mvc
Imports Gallio.Framework
Imports MbUnit.Framework
Imports $mvcprojectnamespace$
Imports $mvcprojectnamespace$.Controllers

<TestFixture()> _
Public Class HomeControllerTest
    <Test()> _
    Public Sub Index()
        ' Setup
        Dim controller As HomeController = New HomeController()

        ' Execute
        Dim result As ViewResult = controller.Index()

        ' Verify
        Dim viewData As ViewDataDictionary = result.ViewData

        Assert.AreEqual("Home Page", viewData("Title"))
        Assert.AreEqual("Welcome to ASP.NET MVC!", viewData("Message"))
    End Sub

    <Test()> _
    Public Sub About()
        ' Setup
        Dim controller As HomeController = New HomeController()

        ' Execute
        Dim result As ViewResult = controller.About()

        ' Verify
        Dim viewData As ViewDataDictionary = result.ViewData

        Assert.AreEqual("About Page", viewData("Title"))
    End Sub
End Class
