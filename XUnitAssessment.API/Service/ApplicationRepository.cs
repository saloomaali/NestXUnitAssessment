﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using XUnitAssessment.API.Data;
using XUnitAssessment.API.Models;

namespace XUnitAssessment.API.Service
{
    public class ApplicationRepository: Interface
    {
        private readonly ApplicationDbContext _dbContext;
        public ApplicationRepository(ApplicationDbContext dbContext) {

            _dbContext = dbContext;
            
        }

        public async Task<Form> ExistingForm(Field? newField)
        {
            
                var existingForm = await _dbContext.Form.Where(x => x.Id == newField.FormId).FirstOrDefaultAsync(x => x.TableId != null);

                if (existingForm != null)
                {
                    return existingForm;
                }
                return null;
            
           
        }
        public async Task<AOColumn> AddField(Field? newField,Form? existingForm)
        {
            var existingColumn = await _dbContext.AOColumn.FirstOrDefaultAsync(x => x.Id == newField.ColumnId && x.TableId == existingForm.TableId);
            if (existingColumn != null)
            {
                await _dbContext.Field.AddAsync(newField);
                await _dbContext.SaveChangesAsync();
                return existingColumn;
            }
            return null;

        }
        public async Task<Field> EditField(Guid id, Field? updatedField)
        {
            var existingField = await _dbContext.Field.FirstOrDefaultAsync(x => x.Id == id);
            if (existingField == null)
            {
                return null;

            }
            existingField.AddChangeDeleteFlag = updatedField.AddChangeDeleteFlag;
            existingField.Sequence = updatedField.Sequence;
            existingField.Type = updatedField.Type;
            existingField.TextAreaRows = updatedField.TextAreaRows;
            existingField.TextAreaCols = updatedField.TextAreaCols;
            existingField.Label = updatedField.Label;
            existingField.DisplayColumns = updatedField.DisplayColumns;

            await _dbContext.SaveChangesAsync();
            return existingField;

        }

        public async Task<Field> ExistingField(Guid id)
        {

            var existing = await _dbContext.Field.FindAsync(id);
            if (existing != null)
            {
                return existing;
            }
            return null;

        }

        public async Task<Field> DeleteField(Field? existing)
        {
            _dbContext.Field.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return existing;
        }


        public async Task<List<Field>> GetFieldByType(string type)
        {
            var fields = await _dbContext.Field.Where(x => x.Type == type).ToListAsync();
            return fields;

        }

        public async Task<List<Field>> FieldByFormName(string formName)
        {
            var fieldsByName = await _dbContext.Field.Where(x => x.form.Name == formName).ToListAsync();
            if (fieldsByName != null)
            {
                return fieldsByName;

            }
            return null;
        }

        public async Task<IActionResult> FieldByFormId(Guid formId)
        {
            var result = await _dbContext.Field.Where(x => x.FormId == formId)
                                                            .Include(x => x.form)
                                                            .GroupBy(x => x.form.Name)
                                                            .Select(group => new
                                                            {
                                                                FormName = group.Key,
                                                                Fields = group.ToList()
                                                            }).ToListAsync();
            if (result != null)
            {
                return new ObjectResult(result);
            }
            return null;
        }

       

        
    }
}