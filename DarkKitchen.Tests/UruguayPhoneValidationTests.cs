using System.Reflection;
using DarkKitchen.Domain.Users;
using DarkKitchen.IBusinessLogic.IAuth;
using DarkKitchen.Models.DTOs;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.Tests;

[TestClass]
public class UruguayPhoneValidationTests
{
    [TestMethod]
    public void IsValid_UruguayMobile_ReturnsTrue()
    {
        var strategy = new UruguayPhoneValidationStrategy(); 
        var phone = "094123456";
        
        var res = strategy.IsValid(phone);
        
        Assert.IsTrue(res);
    }
}
