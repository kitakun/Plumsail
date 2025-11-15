export type FileData = {
  name: string;
  size: number;
  type: string;
};

export type FileRecord = {
  id: string;
  fileData: FileData;
  preSign?: string;
  payload: {
    Description?: string;
    Status?: SubmissionStatus;
    CreatedDate?: string;
    Priority?: PriorityLevel;
    IsPublic?: boolean;
    [key: string]: any;
  };
};

export type FileSubmission = {
  file: File;
  description?: string;
  payload?: string;
  status?: SubmissionStatus;
  priority?: PriorityLevel;
  isPublic?: boolean;
  createdDate?: Date;
};

export type SubmissionResponse = {
  [fileName: string]: string;
};

export type SubmissionStatus = 'Pending' | 'InReview' | 'Approved' | 'Rejected';
export type PriorityLevel = 'Low' | 'Medium' | 'High' | 'Critical';

