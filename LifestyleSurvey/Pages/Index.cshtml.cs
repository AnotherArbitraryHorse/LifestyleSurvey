using LifestyleSurvey;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace LifestyleSurvey.Pages;

public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger;

    private static string ApiToken = "dummy";

    private static HttpClient T2NHSLookupClient = new HttpClient(); // Could do with application of resilience options here

    [BindProperty(SupportsGet = true)]
    public string Message { get; set; } = "whoops";

    public static void InitialiseClient(string path)
    {
        ApiToken = System.IO.File.ReadAllText(path);
        //  If time permitted, I would be DECODING this here. 
        //  Leaving tokens visible is bad practice.
        T2NHSLookupClient.BaseAddress = new Uri($"https://al-tech-test-apim.azure-api.net/tech-test/t2/patients");
        T2NHSLookupClient.DefaultRequestHeaders.Accept.Clear();
        T2NHSLookupClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        T2NHSLookupClient.DefaultRequestHeaders.Add("User-Agent", "T2Razor");
        T2NHSLookupClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiToken);
    }

    /// <summary>
    /// Trival GET - only asynchronous for symmetry
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> OnGetAsync()
    {
        Message = ""; // Clear any old message.
        return Page();
    }

    /// <summary>
    /// Login form POST request received.
    /// This is the part where we provide the "PartOne" functiionality.
    /// The following form variables will be posted:
    /// nhsnumber, surname, dob
    /// 
    /// We collect these as string values. The surname and date of birth values will
    /// be checked against results of the "NHS" lookup API.
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {

        // There should be no repeated values like surname=Highly&surname=Unlikely
        string nhsNumber = Request.Form["nhsno"].ToString();
        string surname = Request.Form["surname"].ToString();
        string dateOfBirth = Request.Form["dob"].ToString();

        string lookupUri = $"{T2NHSLookupClient.BaseAddress}/{nhsNumber}";
        HttpResponseMessage response = await T2NHSLookupClient.GetAsync(lookupUri);
        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            NHSPartialPatientDetails? patientDetails = JsonSerializer.Deserialize<NHSPartialPatientDetails>(jsonResponse);
            if (patientDetails != null && patientDetails.Match(nhsNumber, surname, dateOfBirth))
            {
                int age = patientDetails.Age();
                if (Questionnaire.TheQuestionnaire!.Ineligible(age))
                {
                    Message = "You are not eligible for this service";
                    return Page();
                }

                return RedirectToPage("/Survey"); 
            }
        }

        Message = "Your details could not be found";
        return Page();
    }

}

