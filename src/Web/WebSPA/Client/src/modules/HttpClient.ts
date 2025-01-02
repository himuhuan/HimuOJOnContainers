/**
 * HttpClient.ts
 * This file is used to make http requests to the server.
 */

import axios, {
	AxiosHeaders,
	type AxiosInstance,
	type AxiosResponse,
} from "axios";

// Specific details will be handled by BFF
class HttpClient {
	instance: AxiosInstance;

	constructor() {
		this.instance = axios.create({
			timeout: 5000,
			headers: new AxiosHeaders({
				"X-CSRF": "1",
			}),
		});
	}

	public async get<T>(url: string, data?: any): Promise<AxiosResponse<T>> {
		return this.instance.get<T>(url, data).catch((res) => {
			if (res && res.response) {
				return Promise.reject(res.response);
			} else {
				console.error("Web server is not responding. Please try again later.");
				throw res;
			}
		});
	}

	public async post<T>(url: string, data?: any): Promise<AxiosResponse<T>> {
		return this.instance.post<T>(url, data).catch((res) => {
			if (res && res.response) {
				return Promise.reject(res.response);
			} else {
				console.error("Web server is not responding. Please try again later.");
				throw res;
			}
		});
	}

	public async put<T>(url: string, data?: any): Promise<AxiosResponse<T>> {
		return this.instance.put<T>(url, data).catch((res) => {
			if (res && res.response) {
				return Promise.reject(res.response);
			} else {
				console.error("Web server is not responding. Please try again later.");
				throw res;
			}
		});
	}

	public async delete<T>(url: string, data?: any): Promise<AxiosResponse<T>> {
		return this.instance.delete<T>(url, data).catch((res) => {
			if (res && res.response) {
				return Promise.reject(res.response);
			} else {
				console.error("Web server is not responding. Please try again later.");
				throw res;
			}
		});
	}
}

const client = new HttpClient();

client.instance.interceptors.response.use(
	(response) => {
		return response;
	},
	(error) => {
		if (error.response && error.response.status === 401) {
			window.$message.error("身份验证失败，请重新登录");
		}
		return Promise.reject(error);
	}
);

export default client;
