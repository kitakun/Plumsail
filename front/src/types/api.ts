export type Pagination<T> = {
  items: T[];
  totalCount: number;
};

export type OperationResult<T> = {
  result: T;
  isSuccess: boolean;
  exception?: string;
};

