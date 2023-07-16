using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    public interface IPersonService
    {
        /// <summary>
        /// Adds a person object to the list of peolpe
        /// </summary>
        /// <param name="personToAdd">Person object to add</param>
        /// <returns>The person object after adding it with new Id</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personToAdd);

        /// <summary>
        /// Returns a specific person by Id
        /// </summary>
        /// <param name="personId></param>
        /// <returns>Matching person for a specific personId</returns>
        Task<PersonResponse?> GetPersonById(Guid? personId);

        /// <summary>
        /// Returns all people
        /// </summary>
        /// <returns>List of all people as List<PeopleResponse></returns></returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Returns all people that matches with a given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field do search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching persons bases on search field and search string</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// Returns all the people sorted by a specific field and order specified
        /// </summary>
        /// <param name="persons">List os persons</param>
        /// <param name="sortBy">Field to sort by</param>
        /// <param name="sortOrder">The order option to sort by</param>
        /// <returns>All the persons sorted by the selected field and sort order</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> persons, string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Updates a specific person based on it's id
        /// </summary>
        /// <param name="personUpdateRequest">The person to update</param>
        /// <returns>The person response updated</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// Deletes a specific person based on the id
        /// </summary>
        /// <param name="id">The id of the specific person</param>
        /// <returns>If deleted returns true, else returns false</returns>
        Task<bool> DeletePerson(Guid? id);

        /// <summary>
        /// Returns persons as CSV file
        /// </summary>
        /// <returns>The memory stream with csv data</returns>
        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// Returns persons as Excel file
        /// </summary>
        /// <returns>The memory stream with excel data</returns>
        Task<MemoryStream> GetPersonsExcel(); 
    }
}
