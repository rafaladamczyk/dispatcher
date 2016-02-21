using System;
using System.Linq;
using System.Web.Http.Results;
using Dispatcher.Controllers;
using Dispatcher.Models;
using NSubstitute;
using NUnit.Framework;

namespace Dispatcher.Tests.Controllers
{
    [TestFixture]
    public class DispatchRequestControllerTest
    {
        [Test]
        public void CreationOfNewRequest_ShouldReturnBadRequest_WhenRequesterIdIsInvalid()
        {
            // Arrange
            var controller = new DispatchRequestController(new TestContext());

            // Act
            var result = controller.CreateNewRequest(0, 0).Result;

            // Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
            Assert.That(((BadRequestErrorMessageResult)result).Message, Is.Not.Empty);
        }

        [Test]
        public void CreationOfNewRequest_ShouldReturnNewlyCreatedRequest_WhenRequesterIdIsCorrect()
        {
            var context = new TestContext();
            context.Requesters.Add(new DispatchRequester { Id = 0, Name = "test" });
            var controller = new DispatchRequestController(context);

            var result = (CreatedAtRouteNegotiatedContentResult< DispatchRequest >) controller.CreateNewRequest(0, 0).Result;

            Assert.AreEqual(result.RouteName, "DefaultApi");
            Assert.AreEqual(result.RouteValues["id"], result.Content.Id);
            Assert.AreEqual(result.Content.RequesterId, 0);
            Assert.AreEqual(result.Content.Type, RequestType.BringMaterials);
        }

        [Test]
        public void CreationOfNewRequest_ShouldReturnBadRequest_WhenRequesterIdIsCorrect_ButRequestTypeIsInvalid()
        {
            var context = new TestContext();
            context.Requesters.Add(new DispatchRequester { Id = 0, Name = "test" });
            var controller = new DispatchRequestController(context);

            var result = controller.CreateNewRequest(0, 99).Result;

            // Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
            Assert.That(((BadRequestErrorMessageResult)result).Message, Is.Not.Empty);
        }

        [Test]
        public void CreationOfNewRequest_ShouldFail_IfThereAlreadyExists_AnActiveRequest_WithTheSame_Type_AND_RequesterId()
        {
            var context = new TestContext();
            context.Requesters.Add(new DispatchRequester { Id = 0, Name = "test" });
            var controller = new DispatchRequestController(context);

            var result1 = controller.CreateNewRequest(0, 1).Result;
            var result2 = controller.CreateNewRequest(0, 1).Result;

            // Assert
            Assert.IsInstanceOf<CreatedAtRouteNegotiatedContentResult<DispatchRequest>>(result1);
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result2);
            Assert.That(((BadRequestErrorMessageResult)result2).Message, Is.Not.Empty);
        }

        [Test]
        public void CompletingARequest_ShouldSetItsActiveFlagToFalse_And_SetItsCompletionDate()
        {
            var context = new TestContext();
            context.Requesters.Add(new DispatchRequester { Id = 0, Name = "test" });
            var request = new DispatchRequest { Active = true, CreationDate = DateTime.UtcNow, Id = 0, ProvidingUserName = ""};
            context.Requests.Add(request);
            var controller = new DispatchRequestController(context);

            var result = controller.CompleteRequest(0).Result;

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            Assert.That(request.Active, Is.False);
            Assert.That(request.CompletionDate, Is.Not.Null);
        }

        [Test]
        public void CompletingRequest_ShouldThrow_WhenReuqestIsAlreadyCompleted()
        {
            var context = new TestContext();
            context.Requesters.Add(new DispatchRequester { Id = 0, Name = "test" });
            var request = new DispatchRequest { Active = false, CreationDate = DateTime.UtcNow, Id = 0 };
            context.Requests.Add(request);
            var controller = new DispatchRequestController(context);

            var result = controller.CompleteRequest(0).Result;

            // Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
            Assert.That(((BadRequestErrorMessageResult)result).Message, Is.Not.Empty);
        }

        [Test]
        public void Getting_ActiveRequest_ByRequesterId_ShouldReturnStrippedRequest()
        {
            var context = new TestContext();
            context.Requesters.Add(new DispatchRequester { Id = 7, Name = "test" });
            var request = new DispatchRequest { Active = true, CreationDate = DateTime.UtcNow, Id = 0, RequesterId = 7, Type = RequestType.TakeAwayProduct};
            context.Requests.Add(request);
            var controller = new DispatchRequestController(context);

            var result = ((OkNegotiatedContentResult<StrippedRequest>)controller.GetActiveRequestStripped(7)).Content;

            // Assert
            Assert.That(result.R0 == false);
            Assert.That(result.R1 == true);
            Assert.That(result.R0P == false);
            Assert.That(result.R1P == false);
        }
    }
}
