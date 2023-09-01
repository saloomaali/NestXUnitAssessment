using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XUnitAssessment.API.Models;

namespace XUnitAssessment.API.Service
{
    public interface Interface
    {
        public Task<Form> ExistingForm(Field? newField);
        public Task<AOColumn> AddField(Field? newField, Form? existingForm);
        public Task<Field> EditField(Guid id, Field? updatedField);
        public Task<Field> ExistingField(Guid id);
        public Task<Field> DeleteField(Field? existing);
        public Task<List<Field>> GetFieldByType(string type);
        public Task<List<Field>> FieldByFormName(string formName);
        public Task<IActionResult> FieldByFormId(Guid formId);

    }
}
