import axios from 'axios';
import type { FileRecord } from '../../Submissions/submissions.types';
import type { Pagination, OperationResult } from '../../types/api';

const API_BASE_URL = 'https://localhost:7274/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const signupApi = {
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

  async submitSignUpForm(formData: FormData): Promise<OperationResult<{ [key: string]: string }>> {
    const response = await apiClient.put<OperationResult<{ [key: string]: string }>>(
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

