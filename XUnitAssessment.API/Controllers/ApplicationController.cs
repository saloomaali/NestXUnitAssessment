using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using XUnitAssessment.API.Data;
using XUnitAssessment.API.Models;
using XUnitAssessment.API.Service;

namespace XUnitAssessment.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController:ControllerBase
   
    {
  
        private readonly Interface _Interface;

        public ApplicationController(Interface @interface)
        {
            _Interface = @interface;   
        }

        //Add record in Field table for an existing Form using an existing column and table associated with the Form.

        [HttpPost]
        public async Task<IActionResult> AddRecord(Field? newField)
        {
            try
            {
                
                var existingForm = await _Interface.ExistingForm(newField);
                if (existingForm != null)
                {

                    var existingColumn = await _Interface.AddField(newField, existingForm);
                    if (existingColumn != null)
                    {
                        return Ok("Success");
                    }
                    return BadRequest("No Coulumn exists with given columnId");
                }
                return NotFound("No Form available with given FormId");
            }
            catch (Exception ex)
            {
                 return BadRequest(ex.Message);
            }

        }

        //Edit a record in Field table by passing Id as parameter
       

        [HttpPut]
        public async Task<IActionResult> EditRecord(Guid id, [FromBody] Field updatedField)
        {
            try
            {
                var existingField = await _Interface.EditField(id, updatedField);
                if (existingField != null)
                {
                    return Ok("Success");

                }

                return NotFound("No such field exists");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Delete a record from Field table by passing Id as parameter
        [HttpDelete]
        public async Task<IActionResult> DeleteRecord(Guid id)
        {
            try
            {
                
                var existing = await _Interface.ExistingField(id);
                if (existing == null)
                {
                    return NotFound("No Such Field exists");
                }

                var deleted = await _Interface.DeleteField(existing);
                if (deleted != null)
                {
                    return Ok("Field Deleted");
                }
                return BadRequest("Can not Delete. SomethingWent wrong!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Get all records of Type “Radio” from Field table. Type needs to be defined as a parameter.
        [HttpGet]
        public async Task<IActionResult> GetFieldByType(string type)
        {
            try
            {
                var fields = await _Interface.GetFieldByType(type);
                if (fields != null)
                {
                    return Ok(fields);
                }
                return NotFound("No Field exists with given type");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //Get all records from Field table by passing Form Name as parameter
        [HttpGet]
        [Route("{formName}")]
        public async Task<IActionResult> GetFieldByFormName([FromRoute] string formName)
        {
            try
            {
                var fieldsByName = await _Interface.FieldByFormName(formName);
                if (fieldsByName != null)
                {
                    return Ok(fieldsByName);
                    
                }
                return NotFound("No field existing with the given name");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Get all records from Field table, and Name from Form table, by passing FormId as parameter

        [HttpGet]
        [Route("{formId:Guid}")]
        
        public async Task<IActionResult> GetFields([FromRoute] Guid formId)
        {
            try
            {
                var result = await _Interface.FieldByFormId(formId);

                if (result != null)
                {
                    return Ok(result);

                }
                return NotFound("No Field exist for given FormId");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
