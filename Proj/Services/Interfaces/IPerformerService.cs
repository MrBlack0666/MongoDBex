using mongoDB.Models;
using MongoDB.Bson;
using System.Collections.Generic;

namespace mongoDB.Services
{
    /// <summary>
    /// Interfejs serwisu wykonawców
    /// </summary>
    public interface IPerformerService
    {
        /// <summary>
        /// Metoda udostępniająca dodawanie wykonawcy
        /// </summary>
        /// <param name="performer">Szczegóły wykonawcy</param>
        /// <returns>Zwraca true jeżeli dodało do bazy</returns>
        /// <exception cref="Exceptions.ValidationException">Wyrzuca wyjątek gdy szczegóły nie są prawidłowe</exception>
        bool AddPerformer(Performer performer);
        /// <summary>
        /// Metoda udostępnia usuwanie wykonawców
        /// </summary>
        /// <param name="id">Identyfikator wykonawcy</param>
        /// <returns>Zwraca true jeżeli usunięto z bazy</returns>
        bool DeletePerformer(ObjectId id);
        /// <summary>
        /// Metoda udostępniająca edycję wykonawcy
        /// </summary>
        /// <param name="id"> Identyfikator wykonawcy</param>
        /// <param name="performer">Szczegóły wykonawcy</param>
        /// <returns></returns>
        bool EditPerformer(ObjectId id, Performer performer);
        /// <summary>
        /// Metoda zwracająca wszystkich wykonawców (id, pseudonim) 
        /// </summary>
        /// <returns></returns>
        List<PerformerVM> GetAllPerformers();
        /// <summary>
        /// Metoda zwracająca wykonawców o podanym filtrze
        /// </summary>
        /// <param name="performerFilters">Filtr do wyszukiwania wykonawców</param>
        /// <returns></returns>
        GetPerformersVM GetPerformersWithFilter(PerformerFilters performerFilters);
        /// <summary>
        /// Metoda zwracająca piosenki dla danego wykonawcy
        /// </summary>
        /// <param name="id">Identyfikator wykonawcy</param>
        /// <returns></returns>
        List<SongForPerformer> GetSongsForPerformer(ObjectId id);
    }
}