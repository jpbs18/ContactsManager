using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;
using System.Reflection;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Exceptions;

namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonsRepository _repository;
        private readonly ILogger<PersonService> _logger;

        public PersonService(IPersonsRepository repository, ILogger<PersonService> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        private PersonResponse ConvertPersonToPersonResponse(Person? person)
        {
            PersonResponse response = person.ConvertToPersonResponse();
            response.Country = person?.Country?.Name;

            return response;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personToAdd)
        {
            if(personToAdd is null)
                throw new ArgumentNullException(nameof(personToAdd));
            
            ValidationHelper.ModelValidation(personToAdd);
            Person actualPerson = personToAdd.ConvertToPerson();

            var personsList = await _repository.GetAllPersons();
            var number = personsList.Where(person => person.Name == actualPerson.Name).Count();

            if (number > 0)
                throw new ArgumentException($"{nameof(actualPerson)}: {actualPerson}");

            await _repository.AddPerson(actualPerson);
            return actualPerson.ConvertToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons() 
        {
            _logger.LogInformation("GetAllPersons of PersonsService");

            var persons = await _repository.GetAllPersons();

            return persons.Select(person => person.ConvertToPersonResponse()).ToList(); 
        }


        public async Task<PersonResponse?> GetPersonById(Guid? personId) => personId is null ? null
            : (await _repository.GetPersonById(personId.Value)).ConvertToPersonResponse();

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsService");

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return await GetAllPersons();
            }

            List<Person> persons = new();

            switch (searchBy)
                {
                    case nameof(PersonResponse.Name):
                        persons = await _repository.GetFilteredPersons(person => string.IsNullOrEmpty(person.Name) || person.Name.Contains(searchString!));
                        break;

                    case nameof(PersonResponse.Email):
                        persons = await _repository.GetFilteredPersons(person => string.IsNullOrEmpty(person.Email) || person.Email.Contains(searchString!));
                        break;

                    case nameof(PersonResponse.BirthDate):
                        persons = await _repository.GetFilteredPersons(person => person.BirthDate.Equals(DateTime.Parse(searchString!)));
                        break;

                    case nameof(PersonResponse.Gender):
                        persons = await _repository.GetFilteredPersons(person => string.IsNullOrEmpty(person.Gender) || person.Gender.Contains(searchString!));
                        break;

                    case nameof(PersonResponse.Country):
                        persons = await _repository.GetFilteredPersons(person => string.IsNullOrEmpty(person.Country!.Name) || person.Country.Name.Contains(searchString!));
                        break;

                    case nameof(PersonResponse.Address):
                        persons = await _repository.GetFilteredPersons(person => string.IsNullOrEmpty(person.Address) || person.Address.Contains(searchString!));
                        break;
                }
            
            return persons.Select(person => person.ConvertToPersonResponse()).ToList();
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder)
        {
            _logger.LogInformation("GetSortedPersons of PersonsService");

            if (string.IsNullOrEmpty(sortBy))
            {
                return persons;
            }

            List<PersonResponse> sortedList = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.Name), SortOrderOptions.ASC) =>
                    persons.OrderBy(person => person.Name, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Name), SortOrderOptions.DESC) =>
                    persons.OrderByDescending(person => person.Name, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                    persons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                    persons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.BirthDate), SortOrderOptions.ASC) =>
                   persons.OrderBy(person => person.BirthDate).ToList(),

                (nameof(PersonResponse.BirthDate), SortOrderOptions.DESC) =>
                    persons.OrderByDescending(person => person.BirthDate).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) =>
                    persons.OrderBy(person => person.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) =>
                    persons.OrderByDescending(person => person.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                    persons.OrderBy(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                    persons.OrderByDescending(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
                    persons.OrderBy(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                    persons.OrderByDescending(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                    persons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                    persons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceivedNewsLetter), SortOrderOptions.ASC) =>
                    persons.OrderBy(person => person.ReceivedNewsLetter).ToList(),

                (nameof(PersonResponse.ReceivedNewsLetter), SortOrderOptions.DESC) =>
                    persons.OrderByDescending(person => person.ReceivedNewsLetter).ToList(),

                _ => persons
            };

            return sortedList;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            _logger.LogInformation("Update of PersonsService");

            if (personUpdateRequest is null)
            {
                throw new ArgumentNullException(nameof(Person));
            }
              
            ValidationHelper.ModelValidation(personUpdateRequest);
            Person? actualPersonById = await _repository.GetPersonById(personUpdateRequest.Id);

            if(actualPersonById is null)
            {
                throw new InvalidPersonIdException("Given person does not exist ");
            }              
            
            actualPersonById.Id = personUpdateRequest.Id;
            actualPersonById.Name = personUpdateRequest.Name;
            actualPersonById.Email = personUpdateRequest.Email;
            actualPersonById.Gender = personUpdateRequest.Gender.ToString();
            actualPersonById.BirthDate = personUpdateRequest.BirthDate;
            actualPersonById.Address = personUpdateRequest.Address;
            actualPersonById.CountryId = personUpdateRequest.CountryId;
            actualPersonById.ReceivedNewsLetter = personUpdateRequest.ReceivedNewsLetter;

            await _repository.UpdatePerson(actualPersonById);
            return actualPersonById.ConvertToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? id)
        {
            _logger.LogInformation("DeletePerson of PersonsService");

            if (id is null) 
                throw new ArgumentNullException("Id is empty, please select an Id");
            

            Person? matchingPerson = await _repository.GetPersonById(id.Value);

            if(matchingPerson is null)
                return false;
            
            return await _repository.DeletePerson(matchingPerson.Id);
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWriter, configuration);

            PropertyInfo[] properties = typeof(PersonResponse).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach(var property in properties)
            {
                csvWriter.WriteField(property.Name);
            }
            
            csvWriter.NextRecord();

            List<PersonResponse> persons = await GetAllPersons();

            foreach (var person in persons)
            {
                foreach (var property in properties)
                {
                    var value = property.GetValue(person);

                    if (value is not null && value?.GetType() == typeof(DateTime))
                    {
                        value = ((DateTime) value).ToString("yyyy-MM-dd");
                    }
                    
                    csvWriter.WriteField(value);
                }

                csvWriter.NextRecord();
                csvWriter.Flush();
            }

            memoryStream.Position = 0;
            return memoryStream;

            /* CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);

             csvWriter.WriteHeader<PersonResponse>();
             csvWriter.NextRecord();

             IEnumerable<PersonResponse> persons = await GetAllPersons();
             await csvWriter.WriteRecordsAsync(persons);
             await streamWriter.FlushAsync();

             memoryStream.Position = 0;
             return memoryStream; */
        }

        public async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");

                workSheet.Cells["A1"].Value = "Name";
                workSheet.Cells["B1"].Value = "Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                workSheet.Cells["D1"].Value = "Age";
                workSheet.Cells["E1"].Value = "Gender";
                workSheet.Cells["F1"].Value = "Country";
                workSheet.Cells["G1"].Value = "Address";
                workSheet.Cells["H1"].Value = "Received News Letter";

                int row = 2;
                IEnumerable<PersonResponse> persons = await GetAllPersons();

                foreach (var person in persons)
                {
                    workSheet.Cells[row, 1].Value = person.Name;
                    workSheet.Cells[row, 2].Value = person.Email;

                    if (person.BirthDate.HasValue)
                    {
                        workSheet.Cells[row, 3].Value = person.BirthDate.Value.ToString("yyyy-MM-dd");
                    }
                    
                    workSheet.Cells[row, 4].Value = person.Age;
                    workSheet.Cells[row, 5].Value = person.Gender;
                    workSheet.Cells[row, 6].Value = person.Country;
                    workSheet.Cells[row, 7].Value = person.Address;
                    workSheet.Cells[row, 8].Value = person.ReceivedNewsLetter;

                    row++;
                }

                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();
                await excelPackage.SaveAsAsync(memoryStream);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
