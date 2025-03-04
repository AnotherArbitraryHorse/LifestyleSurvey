using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Numerics;
using System.Text;

namespace LifestyleSurvey.Pages
{
    public class SurveyModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? ClassificationSource { get; set; } // In this case, Age (!)

        [BindProperty(SupportsGet = true)]
        public string? Message { get; set; }

        private readonly ILogger<SurveyModel> _logger;

        public SurveyModel(ILogger<SurveyModel> logger)
        {
            _logger = logger;
            ClassificationSource = "25"; // GIVING UP FOR NOW
            Message = "";
        }

        public void OnGet()
        {
            Message = "";
        }

        public async Task<IActionResult> OnPostAsync()
        { 
            Dictionary<string, string> formContent = new Dictionary<string, string>();
            foreach (var kvp in await Request.ReadFormAsync())
            {
                formContent.Add(kvp.Key,kvp.Value.ToString());
            }
            int score = Questionnaire.TheQuestionnaire!.Score(int.Parse(formContent["ClassificationSource"]),
                    formContent);
            _logger.Log(LogLevel.Information, formContent.ToString() + "=> Score" + score.ToString());
            if (score < 0)
            {
                Message = "Please answer all questions.";
            }
            else if (score >= 4)
            {
                Message = "We think there are some simple things you could do to improve your quality of life, please phone to book an appointment.";
            }
            else
            {
                Message = "Thank you for answering our questions, we don't need to see you at this time. Keep up the good work!";
            }
            return Page();
        }
    }
}
