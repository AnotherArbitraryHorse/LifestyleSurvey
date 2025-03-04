using Microsoft.AspNetCore.Routing.Constraints;
using System.Globalization;
using System.Text.Json.Serialization;

namespace LifestyleSurvey
{ 
    public class NHSPartialPatientDetails
    {
        public required string nhsNumber { get; set; } 
        public required string name { get; set; } 
        public required string born { get; set; }

        
        /// <summary>
        /// Suspicious assumptions here : "Things Programmers Believe About Names" waiting to bite?
        /// </summary>
        public string Surname() => name.Split(',')[0];


        public int Age()
        {
            DateTime whenBorn = DateTime.ParseExact(born, "dd-MM-yyyy", CultureInfo.CurrentCulture);
            DateTime now = DateTime.UtcNow;
            // You don't get a year older until you've passed your birthday.
            return now.Year - whenBorn.Year + ((now.DayOfYear >= whenBorn.DayOfYear) ? 1 : 0);
        }
        

        /// <summary>
        /// Do the user's entered values match our records?
        /// We want an exact match on the NHS number, give or take white space.
        /// Names in the database are recorded as 'SURNAME, other names'
        /// We coerce the trimmed form name to upper case and compare on this.
        /// For the date of birth we replace separator to match the records.
        /// </summary>
        /// <param name="nhsNo">from form</param>
        /// <param name="suppliedSurname">from form</param>
        /// <param name="dateOfBirth">from form</param>
        /// <returns>True iff. compatible</returns>
        internal bool Match(string nhsNo, string suppliedSurname, string dateOfBirth)
        {
            if (!nhsNo.Trim().Equals(nhsNumber))
            {
                return false;
            }
            var s = Surname();
            if (!suppliedSurname.Trim().ToUpper().Equals(Surname()))
            {
                return false;
            }
            // Full test on date of birth.  This may give issues with repeated test sets,
            // as the DoBs in the database are moved backwards to keep subjects at the same age.
            DateTime fromForm = DateTime.ParseExact(dateOfBirth, "yyyy-MM-dd", CultureInfo.CurrentCulture);
            DateTime actual = DateTime.ParseExact(born, "dd-MM-yyyy", CultureInfo.CurrentCulture);

            if (fromForm != actual)
            {
                return false;
            }
            return true; // everything's ok
        }
    }
}
