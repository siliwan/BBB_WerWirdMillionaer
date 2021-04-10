using AutoFixture;
using Backend.Controllers;
using Backend.Data.Models;
using Backend.Data.Models.CRUD;
using Backend.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace Backend_Tests.Controllers
{
    [TestClass]
    public class AnswerControllerTest
    {
        private static readonly Question SampleQuestion = new Question
        {
            Id = 1,
            QuestionText = "TestQuestion",
            Answers = new HashSet<Answer>(),
            Category = new Category(),
            QuestionStatistic = new QuestionStatistic()
        };

        private static readonly Answer SampleAnswer = new Answer
        {
            Id = 1,
            AnswerText = "TestAnswer",
            IsCorrect = true,
            Question = new Question()
        };

        private static readonly List<Answer> AnswerData = new List<Answer>
        {
            SampleAnswer
        };

        [TestMethod]
        public void Get()
        {
            var mockAnswerRepo = new Mock<IAnswerRepository>();
            mockAnswerRepo.Setup(x => x.GetAll()).Returns(AnswerData);
            AnswerController controller = new AnswerController(null, mockAnswerRepo.Object, null);

            var result = controller.Get();

            Assert.AreEqual(AnswerData, result);
        }

        [TestMethod]
        public void GetById()
        {
            var mockAnswerRepo = new Mock<IAnswerRepository>();
            mockAnswerRepo.Setup(x => x.GetById(1)).Returns(SampleAnswer);
            AnswerController controller = new AnswerController(null, mockAnswerRepo.Object, null);

            var result = controller.Get(1);

            Assert.AreEqual(SampleAnswer, result);
        }

        [TestMethod]
        public void Post_Normal()
        {
            var mockAnswerRepo = new Mock<IAnswerRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<object>()));
            var fixture = new Fixture();

            mockAnswerRepo.Setup(x => x.GetById(1)).Returns(SampleAnswer);
            mockQuestionRepo.Setup(x => x.GetById(1)).Returns(SampleQuestion);

            var answer = fixture.Create<string>();
            var isCorrect = fixture.Create<bool>();

            var answerObject = new AnswerObjectWithQuestionReference
            {
                AnswerText = answer,
                IsCorrect = isCorrect,
                QuestionId = SampleQuestion.Id
            };

            AnswerController controller = new AnswerController(null, mockAnswerRepo.Object, mockQuestionRepo.Object);
            controller.ObjectValidator = objectValidator.Object;

            var result = controller.Post(answerObject);

            mockAnswerRepo.Verify(x => x.AddAnswer(SampleQuestion, answer, isCorrect), times: Times.Once());
            mockAnswerRepo.Verify(x => x.SaveChanges(), times: Times.Once());
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void Post_NonExistingQuestion()
        {
            var mockAnswerRepo = new Mock<IAnswerRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<object>()));
            var fixture = new Fixture();

            mockAnswerRepo.Setup(x => x.GetById(1)).Returns(SampleAnswer);
            mockQuestionRepo.Setup(x => x.GetById(SampleQuestion.Id)).Returns<Question>(null);

            var answer = fixture.Create<string>();
            var isCorrect = fixture.Create<bool>();

            var answerObject = new AnswerObjectWithQuestionReference
            {
                AnswerText = answer,
                IsCorrect = isCorrect,
                QuestionId = SampleQuestion.Id
            };

            AnswerController controller = new AnswerController(null, mockAnswerRepo.Object, mockQuestionRepo.Object);
            controller.ObjectValidator = objectValidator.Object;

            var result = controller.Post(answerObject);

            mockAnswerRepo.Verify(x => x.AddAnswer(SampleQuestion, answer, isCorrect), times: Times.Never());
            mockAnswerRepo.Verify(x => x.SaveChanges(), times: Times.Never());
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual($"Question with id {SampleQuestion.Id} does not exist!", (result as BadRequestObjectResult).Value);
        }

        [TestMethod]
        public void Post_ValidationFail()
        {
            var mockAnswerRepo = new Mock<IAnswerRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<object>()));

            var fixture = new Fixture();


            var answer = fixture.Create<string>();
            var isCorrect = fixture.Create<bool>();

            var answerObject = new AnswerObjectWithQuestionReference
            {
                AnswerText = answer,
                IsCorrect = isCorrect,
                QuestionId = SampleQuestion.Id
            };

            mockAnswerRepo.Setup(x => x.GetById(1)).Returns(SampleAnswer);

            AnswerController controller = new AnswerController(null, mockAnswerRepo.Object, mockQuestionRepo.Object);
            controller.ObjectValidator = objectValidator.Object;

            var result = controller.Post(answerObject);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void Put()
        {
            var mockAnswerRepo = new Mock<IAnswerRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<object>()));

            var fixture = new Fixture();

            mockAnswerRepo.Setup(x => x.GetById(1)).Returns(SampleAnswer);
            mockQuestionRepo.Setup(x => x.GetById(SampleQuestion.Id)).Returns<Question>(null);

            var answer = fixture.Create<string>();
            var isCorrect = fixture.Create<bool>();

            var answerObject = new AnswerObjectWithQuestionReference
            {
                AnswerText = answer,
                IsCorrect = isCorrect,
                QuestionId = SampleQuestion.Id
            };

            var answerResult = new Answer
            {
                AnswerText = answer,
                IsCorrect = isCorrect,
            };


            AnswerController controller = new AnswerController(null, mockAnswerRepo.Object, mockQuestionRepo.Object);
            controller.ObjectValidator = objectValidator.Object;

            var result = controller.Put(1, answerObject);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsInstanceOfType((result as OkObjectResult).Value, typeof(Answer));
            Assert.AreEqual(answerResult.AnswerText, ((Answer)(result as OkObjectResult).Value).AnswerText);
            Assert.AreEqual(answerResult.IsCorrect, ((Answer)(result as OkObjectResult).Value).IsCorrect);
        }

        [TestMethod]
        public void Delete()
        {
            var mockAnswerRepo = new Mock<IAnswerRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<object>()));
            var fixture = new Fixture();

            mockAnswerRepo.Setup(x => x.Remove(SampleAnswer));
            mockAnswerRepo.Setup(x => x.GetById(1)).Returns(SampleAnswer);

            AnswerController controller = new AnswerController(null, mockAnswerRepo.Object, mockQuestionRepo.Object);
            controller.ObjectValidator = objectValidator.Object;

            var result = controller.Delete(1);

            mockAnswerRepo.Verify(x => x.Remove(SampleAnswer), times: Times.Once());
            mockAnswerRepo.Verify(x => x.SaveChanges(), times: Times.Once());
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void Delete_NonExisting()
        {
            var mockAnswerRepo = new Mock<IAnswerRepository>();
            var mockQuestionRepo = new Mock<IQuestionRepository>();
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<object>()));
            var fixture = new Fixture();

            mockAnswerRepo.Setup(x => x.Remove(SampleAnswer));
            mockAnswerRepo.Setup(x => x.GetById(1)).Returns<Answer>(null);
            mockQuestionRepo.Setup(x => x.GetById(1)).Returns(SampleQuestion);


            AnswerController controller = new AnswerController(null, mockAnswerRepo.Object, mockQuestionRepo.Object);
            controller.ObjectValidator = objectValidator.Object;

            var result = controller.Delete(1);

            mockAnswerRepo.Verify(x => x.Remove(SampleAnswer), times: Times.Never());
            mockAnswerRepo.Verify(x => x.SaveChanges(), times: Times.Never());
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual($"Answer with id {SampleQuestion.Id} does not exist.", (result as BadRequestObjectResult).Value);
        }
    }
}
