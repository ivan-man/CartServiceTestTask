using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public abstract class PaginationWrapperBase
    {
        /// <summary>
        /// Number of current page.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items on page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Number of pages.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Number of pages.
        /// </summary>
        public int TotalPages
        {
            get => PageSize != 0 ? (int)Math.Ceiling((decimal)TotalItems / PageSize) : 0;
        }
    }

    /// <summary>
    /// Wrapper for GET requests with pagination attributes.
    /// </summary>
    /// <typeparam name="T">Type of requested entities.</typeparam>
    public class PaginationWrapper<T> : PaginationWrapperBase
        where T : class, new()
    {
        private IEnumerable<T> _data;
        /// <summary>
        /// T Entities.
        /// </summary>
        public IEnumerable<T> Data
        {
            get => _data ??= new List<T>();
            set => _data = value;
        }

        public PaginationWrapper(IEnumerable<T> data)
        {
            _data = data;
        }
    }

    /// <summary>
    /// Wrapper for GET requests with pagination attributes.
    /// </summary>
    /// <typeparam name="T">Type of models.</typeparam>
    /// <typeparam name="TViewModel">Type of viewModels.</typeparam>
    public class PaginationWrapper<T, TViewModel> : PaginationWrapperBase
        where T : class, new()
    {
        private IEnumerable<TViewModel> _data;
        /// <summary>
        /// T Entities.
        /// </summary>
        public IEnumerable<TViewModel> Data
        {
            get => _data ??= new List<TViewModel>();
            set => _data = value;
        }

        public PaginationWrapper(PaginationWrapper<T> otherWrapper, Func<T, TViewModel> convertFunc)
        {
            Page = otherWrapper.Page;
            PageSize = otherWrapper.PageSize;
            TotalItems = otherWrapper.TotalItems;

            _data = otherWrapper.Data.Select(q => convertFunc(q));
        }
    }
}
