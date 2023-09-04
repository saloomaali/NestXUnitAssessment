using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Runtime.CompilerServices;
using XUnitAssessment.API.Controllers;
using XUnitAssessment.API.Models;
using XUnitAssessment.API.Service;

namespace XUnitAssessment.API.Tests.ControllerTests
{
    public class ApplicationControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<Interface> _mockInterface;
        private readonly ApplicationController _sut;

        public ApplicationControllerTests()
        {
            _fixture = new Fixture();
            _mockInterface = _fixture.Freeze<Mock<Interface>>();
            _sut = new ApplicationController(_mockInterface.Object);
        }

        //TEST CASES FOR AddRecord()
        //1
        [Fact]
        public async Task AddRecord_ReturnsOk_WhenFormAndColumnExist()
        {
            // Arrange
            var responseField = new Field();
            
            var newField = new Field
            {
                FormId = _fixture.Create<Guid>(),
                ColumnId = _fixture.Create<Guid>(),
            };
            var existingForm = new Form
            {
                Id = (Guid)newField.FormId,
                TableId = _fixture.Create<Guid>(),
            };
            var existingColumn = new AOColumn
            {
                Id = (Guid)newField.ColumnId,
                TableId = existingForm.TableId,
            };

            _mockInterface.Setup(s => s.ExistingForm(newField)).ReturnsAsync(existingForm);
            _mockInterface.Setup(s => s.AddField(newField, existingForm)).ReturnsAsync(existingColumn);

            // Act
            var result = await _sut.AddRecord(newField);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
                

            // Verify that the service methods were called with the correct arguments
            _mockInterface.Verify(s => s.ExistingForm(newField), Times.Once);
            _mockInterface.Verify(s => s.AddField(newField, existingForm), Times.Once);
        }
        //2
        [Fact]
        public async Task AddRecord_ReturnsNotFound_WhenFormNotFound()
        {
            // Arrange
            var newField = new Field
            {
                FormId = _fixture.Create<Guid>(),
                ColumnId = _fixture.Create<Guid>(),
               
            };

            _mockInterface.Setup(s => s.ExistingForm(newField)).ReturnsAsync((Form)null);

            // Act
            var result = await _sut.AddRecord(newField);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("No Form available with given FormId");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.ExistingForm(newField), Times.Once);
        }

        //3
        [Fact]
        public async Task AddRecord_ReturnsBadRequest_WhenColumnNotFound()
        {
            // Arrange
            var newField = new Field
            {
                FormId = _fixture.Create<Guid>(),
                ColumnId = _fixture.Create<Guid>(),
            };
            var existingForm = new Form
            {
                Id = (Guid)newField.FormId,
                TableId = _fixture.Create<Guid>(),
            };

            _mockInterface.Setup(s => s.ExistingForm(newField)).ReturnsAsync(existingForm);
            _mockInterface.Setup(s => s.AddField(newField, existingForm)).ReturnsAsync((AOColumn)null);
            

            // Act
            var result = await _sut.AddRecord(newField);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("No Coulumn exists with given columnId");

            // Verify that the service methods were called with the correct arguments
            _mockInterface.Verify(s => s.ExistingForm(newField), Times.Once);
            _mockInterface.Verify(s => s.AddField(newField, existingForm), Times.Once);
        }

        //4
        [Fact]
        public async Task AddRecord_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var newField = new Field
            {
                FormId = _fixture.Create<Guid>(),
                ColumnId = _fixture.Create<Guid>(),
                
            };
            var existingForm = new Form
            {
                Id = (Guid)newField.FormId,
                TableId = _fixture.Create<Guid>()
               
            };

            _mockInterface.Setup(s => s.ExistingForm(newField)).ReturnsAsync(existingForm);
            _mockInterface.Setup(s => s.AddField(newField, existingForm)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _sut.AddRecord(newField);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Test exception");

            // Verify that the service methods were called with the correct arguments
            _mockInterface.Verify(s => s.ExistingForm(newField), Times.Once);
            _mockInterface.Verify(s => s.AddField(newField, existingForm), Times.Once);
        }
        //TEST CASES FOR EditRecord()
        //1
        [Fact]
        public async Task EditRecord_ReturnsOk_WhenFieldExists()
        {
            // Arrange
            var fieldId = _fixture.Create<Guid>(); 
            var updatedField = new Field
            {
                Id = fieldId,
                
            };
            var existingField = new Field
            {
                Id = fieldId,
                
            };

            _mockInterface.Setup(s => s.EditField(fieldId, updatedField)).ReturnsAsync(existingField);

            // Act
            var result = await _sut.EditRecord(fieldId, updatedField);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be("Success");

            // Verify that the service method was called with the correct arguments
            _mockInterface.Verify(s => s.EditField(fieldId, updatedField), Times.Once);
        
        }

        //2
        [Fact]
        public async Task EditRecord_ReturnsNotFound_WhenFieldNotFound()
        {
            // Arrange
            var fieldId = _fixture.Create<Guid>(); 
            var updatedField = new Field
            {
                Id = fieldId,
                
            };

            _mockInterface.Setup(s => s.EditField(fieldId, updatedField)).ReturnsAsync((Field)null);
            

            // Act
            var result = await _sut.EditRecord(fieldId, updatedField);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("No such field exists");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.EditField(fieldId, updatedField), Times.Once);
        }

        //3
        [Fact]
        public async Task EditRecordn_ReturnsBadRequest_WhenExceptionThrow()
        {
            // Arrange
           
            var fieldId = _fixture.Create<Guid>();
            var updatedField = new Field
            {
                Id = fieldId,
                
            };

            _mockInterface.Setup(s => s.EditField(fieldId, updatedField)).ThrowsAsync(new Exception("Test exception"));
            

            // Act
            var result = await _sut.EditRecord(fieldId, updatedField);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Test exception");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.EditField(fieldId, updatedField), Times.Once);
        }
    


        //TEST CASES FOR DeleteRecord()
        //1
        [Fact]
        public async Task DeleteRecord_SuccessfulDeletion_WhenValidInput()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var existingField = new Field();
            _mockInterface.Setup(x => x.ExistingField(id))
                      .ReturnsAsync(existingField);

            _mockInterface.Setup(x => x.DeleteField(existingField))
                      .ReturnsAsync(existingField);

            // Act
            var result = await _sut.DeleteRecord(id);
    
            // Assert
            result.Should().BeOfType<OkObjectResult>()
              .Subject.Value.Should().Be("Field Deleted");

            // Verify that the methods were called as expected
            _mockInterface.Verify(i => i.ExistingField(id), Times.Once);
            _mockInterface.Verify(i => i.DeleteField(existingField), Times.Once);
            
        }

        //2
        [Fact]
        public async Task DeleteRecord_ReturnsNotFound_WhenFieldNotFound()
        {
            // Arrange
            var id = _fixture.Create<Guid>();

            _mockInterface.Setup(i => i.ExistingField(id)).ReturnsAsync((Field)null);

            

            // Act
            var result = await _sut.DeleteRecord(id);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("No Such Field exists");

            // Verify that the method was called
            _mockInterface.Verify(i => i.ExistingField(id), Times.Once);
            _mockInterface.Verify(i => i.DeleteField(It.IsAny<Field>()), Times.Never);
        }

        //3
        [Fact]
        public async Task DeleteRecord_ReturnsBadRequest_WhenErrorDuringDeletion()
        {
            // Arrange
            var id = _fixture.Create<Guid>();

            var existingField = new Field(); 

            _mockInterface.Setup(i => i.ExistingField(id)).ReturnsAsync(existingField);
            _mockInterface.Setup(i => i.DeleteField(existingField)).ThrowsAsync(new Exception("Error during deletion"));

            

            // Act
            var result = await _sut.DeleteRecord(id);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Error during deletion");

            // Verify that the methods were called
            _mockInterface.Verify(i => i.ExistingField(id), Times.Once);
            _mockInterface.Verify(i => i.DeleteField(existingField), Times.Once);
        }


        //TEST CASES FOR GetFieldByType()
        //1
        [Fact]
        public async Task GetFieldByType_ReturnsOk_WhenFieldsFound()
        {
            // Arrange
            
            var fieldType = _fixture.Create<string>(); // Generate a random field type.
            var expectedFields = new List<Field> { new Field(), new Field() };

            _mockInterface.Setup(s => s.GetFieldByType(fieldType)).ReturnsAsync(expectedFields);

            

            // Act
            var result = await _sut.GetFieldByType(fieldType);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedFields);

            // Verify that the service method was called with the correct parameter
            _mockInterface.Verify(s => s.GetFieldByType(fieldType), Times.Once);
        }

        //2
        [Fact]
        public async Task GetFieldByType_ReturnsNotFound_WhenNoFieldsFound()
        {
            // Arrange
            var fieldType = _fixture.Create<string>(); 
            
            _mockInterface.Setup(s => s.GetFieldByType(fieldType)).ReturnsAsync((List<Field>)null);
           

            // Act
            var result = await _sut.GetFieldByType(fieldType);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("No Field exists with given type");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.GetFieldByType(fieldType), Times.Once);
        }


        //3
        [Fact]
        public async Task GetFieldByType_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var fieldType = _fixture.Create<string>();

            _mockInterface.Setup(s => s.GetFieldByType(fieldType)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _sut.GetFieldByType(fieldType);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Test exception");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.GetFieldByType(fieldType), Times.Once);
        }

        //TEST CASES FOR GetFieldByFormName()
        //1
        [Fact]
        public async Task GetFieldByFormName_ReturnsOk_WhenFieldsFound()
        {
            // Arrange
            var formName = _fixture.Create<string>(); 
            var expectedFields = new List<Field> { new Field(), new Field() };

            _mockInterface.Setup(s => s.FieldByFormName(formName)).ReturnsAsync(expectedFields);

            // Act
            var result = await _sut.GetFieldByFormName(formName);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedFields);

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.FieldByFormName(formName), Times.Once);
        }

        //2
        [Fact]
        public async Task GetFieldByFormName_ReturnsNotFound_WhenNoFieldsFound()
        {
            // Arrange
            var formName = _fixture.Create<string>(); 
            _mockInterface.Setup(s => s.FieldByFormName(formName)).ReturnsAsync((List<Field>)null);

            // Act
            var result = await _sut.GetFieldByFormName(formName);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("No field existing with the given name");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.FieldByFormName(formName), Times.Once);
        }


        //3
        [Fact]
        public async Task GetFieldByFormName_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var formName = _fixture.Create<string>(); 
            _mockInterface.Setup(s => s.FieldByFormName(formName)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _sut.GetFieldByFormName(formName);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Test exception");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.FieldByFormName(formName), Times.Once);
        }


        //TEST CASES FOR GetFields()
        //1
        [Fact]
        public async Task GetFields_ReturnsOk_WhenFieldsFound()
        {
            // Arrange
            var formId = _fixture.Create<Guid>(); 
            
            var response = _fixture.Create<ObjectResult>();
            _mockInterface.Setup(s => s.FieldByFormId(formId)).ReturnsAsync(response);
    
            // Act
            var result = await _sut.GetFields(formId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
                

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.FieldByFormId(formId), Times.Once);
        }


        //2
        [Fact]
        public async Task GetFields_ReturnsNotFound_WhenNoFieldsFound()
        {
            // Arrange
            var formId = _fixture.Create<Guid>();
            
            _mockInterface.Setup(s => s.FieldByFormId(formId)).ReturnsAsync((ObjectResult)null);

            // Act
            var result = await _sut.GetFields(formId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("No Field exist for given FormId");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.FieldByFormId(formId), Times.Once);
        }

        //3
        [Fact]
        public async Task GetFields_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var formId = _fixture.Create<Guid>(); 
            _mockInterface.Setup(s => s.FieldByFormId(formId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _sut.GetFields(formId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Test exception");

            // Verify that the service method was called with the correct argument
            _mockInterface.Verify(s => s.FieldByFormId(formId), Times.Once);
        }


    }
}