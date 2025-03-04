using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace LifestyleSurvey
{
    public class Response
    {
        private XElement definition;
        private Question question;
        private List<ScoreClassification> scoreClassifications;
        public string Text { get; }


        public Response(XElement response, Question parentQuestion)
        {
            definition = response;
            question = parentQuestion;
            Text = definition.Attribute("text")!.Value;
            // initial default. Actual scoring only possible after SetApplicableScores() called.
            scoreClassifications = response.Elements("scoreClassification").
                Select(sc => new ScoreClassification(sc, this)).ToList();
        }

        /// <summary>
        /// The score if this response is selected for
        /// the given classificationSource - Age in this case.
        /// </summary>
        /// <param name="classificationSource">Age, or some other numeric</param>
        /// <returns>Score for selected response</returns>
        internal int ApplicableScore(int classificationSource)
        {
            ScoreClassification? applicable = scoreClassifications.
                FirstOrDefault(sc => sc.ApplicableToSource(classificationSource));
            return applicable?.Score ?? 0;
        }
    }
}