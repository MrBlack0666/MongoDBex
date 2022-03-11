using MongoDB.Bson;
using mongoDB.Exceptions;
using mongoDB.Models;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace mongoDB.Aspects
{
    /// <summary>
    /// Klasa aspektu logująca zmiany w pliku dla piosenek.
    /// </summary>
    [PSerializable]
    public class SongsLogger : OnMethodBoundaryAspect
    {
        private static readonly log4net.ILog log = LogHelper.GetLogger(); //log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Metoda uruchamia się gdy wywyoływana metoda otoczona aspektem wykona się poprawnie. Metoda zapisuje do logów informacje o dodaniu/edycji/usunięciu piosenki oraz o dodaniu/usunięciu oceny.
        /// </summary>
        /// <param name="args">Informacje o wykonywanej funkcji objęte aspektem</param>
        public override void OnSuccess(MethodExecutionArgs args)
        {
            if (args.Method.Name.Contains("Rating"))
            {
                if (args.Method.Name.Contains("Add"))
                {
                    var songId = (ObjectId)args.Arguments.GetArgument(0);
                    var rating = (RatingClass)args.Arguments.GetArgument(1);
                    log.Info(string.Format("Rating [ rating - {0}, review - {1} ] was SUCCESSFULLY ADDED to song with id {2}", rating.Rating, rating.Review, songId.ToString()));
                }
                else if (args.Method.Name.Contains("Delete"))
                {
                    var songId = (ObjectId)args.Arguments.GetArgument(0);
                    var ratingId = (int)args.Arguments.GetArgument(1);
                    log.Info(string.Format("Rating with id {0} was SUCCESSFULLY DELETED from song with id {1}", ratingId, songId.ToString()));
                }
            }
            else if (args.Method.Name.Equals("AddSong"))
            {
                var song = (SongVM)args.Arguments.GetArgument(0);
                log.Info(string.Format("Song with title {0} was SUCCESSFULLY ADDED to database", song.Title));
            }
            else if (args.Method.Name.Equals("EditSong") || args.Method.Name.Equals("DeleteSong"))
            {
                var songId = (ObjectId)args.Arguments.GetArgument(0);
                log.Info(string.Format("Song with id {0} was SUCCESSFULLY {1} database", songId.ToString(), args.Method.Name.Contains("EditSong") ? "EDITED in" : "DELETED from"));
            }
        }

        /// <summary>
        /// Metoda uruchamia się gdy wywyoływana metoda otoczona aspektem wyrzuci wyjątek. Metoda zapisuje do logów informacje o niepowodzeniu
        /// </summary>
        /// <param name="args">Informacje o wykonywanej funkcji objęte aspektem</param>
        public override void OnException(MethodExecutionArgs args)
        {
            var exceptionName = args.Exception.GetType().Name;
            var exceptionMessage = args.Exception.Message;
            if (args.Exception is ValidationException)
            {

                if (args.Method.Name.Equals("AddSong"))
                {
                    var song = (SongVM)args.Arguments.GetArgument(0);
                    log.Warn(string.Format("Song with title {0} WASN'T ADDED to database, exception {1} occurred with message: {2}", song.Title, exceptionName, exceptionMessage));
                    return;
                }
                else if (args.Method.Name.Equals("DeleteSong"))
                {
                    var songId = (ObjectId)args.Arguments.GetArgument(0);
                    log.Warn(string.Format("Song with id {0} WASN'T DELETED from database, exception {1} occurred with message: {2}", songId.ToString(), exceptionName, exceptionMessage));
                    return;
                }
                else if (args.Method.Name.Equals("DeleteRatingFromSong"))
                {
                    var songId = (ObjectId)args.Arguments.GetArgument(0);
                    var rating = (RatingClass)args.Arguments.GetArgument(1);
                    log.Warn(string.Format("Rating [ rating - {0}, review - {1} ] WASN'T ADDED to song with id {2}, exception {3} occurred with message: {4}", rating.Rating, rating.Review, songId.ToString(), exceptionName, exceptionMessage));
                    return;
                }
            }
            
            log.Error($"Undefined exception occurred - {exceptionName}  with message: {args.Exception.Message}");
        }
    }
}
