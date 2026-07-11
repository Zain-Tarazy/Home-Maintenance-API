namespace HomeMaintenanceAPI.Application.Common
{
    public class PaginationParams
    {
        private const int MaxPageSize = 1000;

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public bool IsPaginationEnabled =>
            PageNumber.HasValue || PageSize.HasValue;

        public int GetPageNumber()
        {
            if (!PageNumber.HasValue || PageNumber.Value < 1)
                return 1;

            return PageNumber.Value;
        }

        public int GetPageSize()
        {
            if (!PageSize.HasValue || PageSize.Value < 1)
                return 10;

            if (PageSize.Value > MaxPageSize)
                return MaxPageSize;

            return PageSize.Value;
        }
    }
}
