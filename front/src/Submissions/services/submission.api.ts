import axios from 'axios';
import type { FileRecord, FileSubmission, SubmissionResponse } from '../submissions.types';
import type { Pagination, OperationResult } from '../../types/api';

const API_BASE_URL = 'https://localhost:7274/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const submissionApi = {
  async getSubmissions(offset?: number, limit?: number): Promise<OperationResult<Pagination<FileRecord>>> {
    const params = new URLSearchParams();
    if (offset !== undefined) params.append('offset', offset.toString());
    if (limit !== undefined) params.append('limit', limit.toString());
    
    const response = await apiClient.get<OperationResult<Pagination<FileRecord>>>(
      `/submission?${params.toString()}`
    );
    return response.data;
  },

  async searchSubmissions(searchTerm: string, offset?: number, limit?: number): Promise<OperationResult<Pagination<FileRecord>>> {
    const params = new URLSearchParams();
    params.append('searchTerm', searchTerm);
    if (offset !== undefined) params.append('offset', offset.toString());
    if (limit !== undefined) params.append('limit', limit.toString());
    
    const response = await apiClient.get<OperationResult<Pagination<FileRecord>>>(
      `/submission/search?${params.toString()}`
    );
    return response.data;
  },

  async submitFiles(files: FileSubmission[]): Promise<OperationResult<SubmissionResponse>> {
    const formData = new FormData();
    
    files.forEach((fileSubmission, index) => {
      formData.append(`files[${index}].File`, fileSubmission.file);
      if (fileSubmission.description) {
        formData.append(`files[${index}].Description`, fileSubmission.description);
      }
      if (fileSubmission.unknownProperty) {
        formData.append(`files[${index}].UnknownProperty`, fileSubmission.unknownProperty);
      }
      if (fileSubmission.status) {
        formData.append(`files[${index}].Status`, fileSubmission.status);
      }
      if (fileSubmission.priority) {
        formData.append(`files[${index}].Priority`, fileSubmission.priority);
      }
      if (fileSubmission.isPublic !== undefined) {
        formData.append(`files[${index}].IsPublic`, fileSubmission.isPublic.toString());
      }
      if (fileSubmission.createdDate) {
        formData.append(`files[${index}].CreatedDate`, fileSubmission.createdDate.toISOString());
      }
    });

    const response = await apiClient.put<OperationResult<SubmissionResponse>>(
      '/submission',
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  },
};

