export type FileRecord = {
  id: string;
  name: string;
  size: number;
  type: string;
  preSign?: string;
  description?: string;
  status?: SubmissionStatus;
  createdDate?: string;
  priority?: PriorityLevel;
  isPublic?: boolean;
};

export type FileSubmission = {
  file: File;
  description?: string;
  unknownProperty?: string;
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

