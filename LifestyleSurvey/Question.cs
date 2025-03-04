using System.Reflection;
using System.Xml.Linq;

namespace LifestyleSurvey
{
    public class Question
    {
        private Questionnaire parent; // Possibly unnecessary at this stage, but generally useful practice
        private XElement definition; // 

        public string Id { get; }
        public string Text { get; }

        public List<Response> Responses { get; }

        /// <summary>
        /// Construct a Question object within a Questionnaire on the basis of a defining XML question element.
        /// A question element contains:
        /// + An 'id' attribute - "Q2", for example
        /// + A text element containing the body of the question
        /// + Multiple response elements for the possible answers.
        /// + Each response is defined by a text attribute.
        /// + A response also provides a number of 'scoreClassification' elements defining the score to be used
        /// if the classification is applicable. The conditions are defined by the madnatory 'source' attribute
        /// (always 'Age' for this release) and optional 'GEQ' and 'LEQ' attributes containing integer values
        /// (GEQ stands for greater than or equal to; LEQ for less than or equal to).  The value of the score itself is
        /// given in the body of the element.
        /// <param name="question">definition of the question, its responses, and possible scores.</param>
        /// <param name="questionnaire">to which this question belongs</param>
        public Question(XElement question, Questionnaire questionnaire)
        {
            definition = question;
            parent = questionnaire;
            Id = definition.Attribute("id")!.Value;
            Text = definition.Element("text")!.Value;
            Responses = question.Elements("response").
                Select(r => new Response(r, this)).ToList();
        }
    }
}