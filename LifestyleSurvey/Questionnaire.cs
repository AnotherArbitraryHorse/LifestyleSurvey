using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LifestyleSurvey
{
    public class Questionnaire
    {
        public Boolean IsValid { get; }
        public string DefinitionPath { get; }
        public string Title { get; set; }
        public  List<Question> Questions { get; set; }
        private string? source; // This will be Age, as given away below.
        private int? minimumAge;
        private int? maximumAge;

        public static Questionnaire? TheQuestionnaire { get; private set; }

        internal Questionnaire(string definitionPath)
        {
            DefinitionPath = definitionPath;
            Questions = new List<Question>();
            Title = definitionPath + " is invalid";
            IsValid = false;
            try
            {   // Load definition and set up properties, questions, their responses, and score classifications.  
                XDocument qudoc = XDocument.Load(definitionPath);
                XElement root = qudoc.Root!;
                

                // Base questionnaire eligibility on a named attribute of the patient.
                // In fact, it MUST be "Age" at this point - 
                // we don't have enough patient data to be able to deal with anything else.
                // This just gestures towards the possibility of more sophisticated information gathering.
                source = root.Attribute("eligibilitySource")!.Value;
                minimumAge = OptionalIntAttribute(root, "GEQ");
                maximumAge = OptionalIntAttribute(root, "LEQ");
    
                Questions = root.Elements("question").
                    Select(q => new Question(q, this)).ToList();
                // Could usefully enhance this with more checks of questionnaire validity here - 
                // tests such as id uniqueness, response uniqueness within a question, and "coverage" of
                // scoreClassification elements - which ought to provide partitioning of their source domain.
                // However, we CAN assume these conditions to be met at this point, since we are not yet
                // providing a full questionnaire content management system.
                Title = root.Attribute("title")!.Value!;
                IsValid = true;
            }
            catch
            {   // For this exercise just report a generic "invalid questionnaire" message -
                // we're using a valid one to provide the required functionality. 
                // Obviously, improved error checking and/or a questionnaire editor
                // would be useful additions. But I am limited for time here, as this is the first ASP.NET app
                // I've ever written.
                Questions = new List<Question>();
                Title = definitionPath + " is invalid";
                IsValid = false;
            }
        }

        /// <summary>
        /// Read the attribute's value and convert it to an integer value if present, otherwise returning null.
        /// </summary>
        /// <param name="element">element of questionnarie XML</param>
        /// <param name="attributeName">an optional attribute, which MUST be an integer</param>
        /// <returns>attribute value as int - null if not present</returns>
        internal static int? OptionalIntAttribute(XElement root, string attributeName)
        {
            string? intAsString = root.Attribute(attributeName)?.Value;
            if (intAsString == null)
            {   // No attribute
                return null;
            }
            // Note : we just throw an exception if the XML is invalid at this point.
            return int.Parse(intAsString);
        }

        internal static void Load(string questionnairePath)
        {
            TheQuestionnaire = new Questionnaire(questionnairePath);
        }

        internal bool Ineligible(int age)
        {
            if (minimumAge != null && age < minimumAge)
            {
                return true;
            }
            if (maximumAge != null && age > maximumAge)
            {
                return true;
            }
            return false;
        }

        internal  int Score(int classificationSource, Dictionary<string, string> formContent)
        {
            int score = 0;
            foreach (Question q in Questions)
            {
                string answer = formContent[q.Id];
                Response? reply = q.Responses.FirstOrDefault(r => r.Text.Equals(answer));
                if (reply != null)
                {
                    score += reply.ApplicableScore(classificationSource);
                }
                else
                {   // No answer selected???
                    return -1;
                }
            }
            return score;
        }
    }
}