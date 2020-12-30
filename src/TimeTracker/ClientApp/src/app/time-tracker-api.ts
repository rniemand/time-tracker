/* tslint:disable */
/* eslint-disable */
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.9.4.0 (NJsonSchema v10.3.1.0 (Newtonsoft.Json v12.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------
// ReSharper disable InconsistentNaming

import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { Observable, throwError as _observableThrow, of as _observableOf } from 'rxjs';
import { Injectable, Inject, Optional, InjectionToken } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';

export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL');

export interface IAuthClient {
    authenticate(request: AuthenticationRequest): Observable<AuthenticationResponse>;
    locked(): Observable<string>;
}

@Injectable()
export class AuthClient implements IAuthClient {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    authenticate(request: AuthenticationRequest): Observable<AuthenticationResponse> {
        let url_ = this.baseUrl + "/api/Auth/authenticate";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(request);

        let options_ : any = {
            body: content_,
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Content-Type": "application/json",
                "Accept": "application/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processAuthenticate(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processAuthenticate(<any>response_);
                } catch (e) {
                    return <Observable<AuthenticationResponse>><any>_observableThrow(e);
                }
            } else
                return <Observable<AuthenticationResponse>><any>_observableThrow(response_);
        }));
    }

    protected processAuthenticate(response: HttpResponseBase): Observable<AuthenticationResponse> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }}
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = AuthenticationResponse.fromJS(resultData200);
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<AuthenticationResponse>(<any>null);
    }

    locked(): Observable<string> {
        let url_ = this.baseUrl + "/api/Auth/locked";
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processLocked(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processLocked(<any>response_);
                } catch (e) {
                    return <Observable<string>><any>_observableThrow(e);
                }
            } else
                return <Observable<string>><any>_observableThrow(response_);
        }));
    }

    protected processLocked(response: HttpResponseBase): Observable<string> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }}
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = resultData200 !== undefined ? resultData200 : <any>null;
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<string>(<any>null);
    }
}

export interface IClientsClient {
    getAllClients(test: DerivedBaseApiRequest): Observable<ClientDto[]>;
}

@Injectable()
export class ClientsClient implements IClientsClient {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    getAllClients(test: DerivedBaseApiRequest): Observable<ClientDto[]> {
        let url_ = this.baseUrl + "/api/Clients";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(test);

        let options_ : any = {
            body: content_,
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Content-Type": "application/json",
                "Accept": "application/json"
            })
        };

        return this.http.request("post", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processGetAllClients(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processGetAllClients(<any>response_);
                } catch (e) {
                    return <Observable<ClientDto[]>><any>_observableThrow(e);
                }
            } else
                return <Observable<ClientDto[]>><any>_observableThrow(response_);
        }));
    }

    protected processGetAllClients(response: HttpResponseBase): Observable<ClientDto[]> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }}
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(ClientDto.fromJS(item));
            }
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<ClientDto[]>(<any>null);
    }
}

export interface IWeatherForecastClient {
    get(): Observable<WeatherForecast[]>;
}

@Injectable()
export class WeatherForecastClient implements IWeatherForecastClient {
    private http: HttpClient;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(@Inject(HttpClient) http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
        this.http = http;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    get(): Observable<WeatherForecast[]> {
        let url_ = this.baseUrl + "/WeatherForecast";
        url_ = url_.replace(/[?&]$/, "");

        let options_ : any = {
            observe: "response",
            responseType: "blob",
            headers: new HttpHeaders({
                "Accept": "application/json"
            })
        };

        return this.http.request("get", url_, options_).pipe(_observableMergeMap((response_ : any) => {
            return this.processGet(response_);
        })).pipe(_observableCatch((response_: any) => {
            if (response_ instanceof HttpResponseBase) {
                try {
                    return this.processGet(<any>response_);
                } catch (e) {
                    return <Observable<WeatherForecast[]>><any>_observableThrow(e);
                }
            } else
                return <Observable<WeatherForecast[]>><any>_observableThrow(response_);
        }));
    }

    protected processGet(response: HttpResponseBase): Observable<WeatherForecast[]> {
        const status = response.status;
        const responseBlob =
            response instanceof HttpResponse ? response.body :
            (<any>response).error instanceof Blob ? (<any>response).error : undefined;

        let _headers: any = {}; if (response.headers) { for (let key of response.headers.keys()) { _headers[key] = response.headers.get(key); }}
        if (status === 200) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(WeatherForecast.fromJS(item));
            }
            return _observableOf(result200);
            }));
        } else if (status !== 200 && status !== 204) {
            return blobToText(responseBlob).pipe(_observableMergeMap(_responseText => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            }));
        }
        return _observableOf<WeatherForecast[]>(<any>null);
    }
}

export class AuthenticationResponse implements IAuthenticationResponse {
    userId?: number;
    username?: string | undefined;
    firstName?: string | undefined;
    lastName?: string | undefined;
    token?: string | undefined;

    constructor(data?: IAuthenticationResponse) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.userId = _data["userId"];
            this.username = _data["username"];
            this.firstName = _data["firstName"];
            this.lastName = _data["lastName"];
            this.token = _data["token"];
        }
    }

    static fromJS(data: any): AuthenticationResponse {
        data = typeof data === 'object' ? data : {};
        let result = new AuthenticationResponse();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["userId"] = this.userId;
        data["username"] = this.username;
        data["firstName"] = this.firstName;
        data["lastName"] = this.lastName;
        data["token"] = this.token;
        return data; 
    }
}

export interface IAuthenticationResponse {
    userId?: number;
    username?: string | undefined;
    firstName?: string | undefined;
    lastName?: string | undefined;
    token?: string | undefined;
}

export class AuthenticationRequest implements IAuthenticationRequest {
    username?: string | undefined;
    password?: string | undefined;

    constructor(data?: IAuthenticationRequest) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.username = _data["username"];
            this.password = _data["password"];
        }
    }

    static fromJS(data: any): AuthenticationRequest {
        data = typeof data === 'object' ? data : {};
        let result = new AuthenticationRequest();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["username"] = this.username;
        data["password"] = this.password;
        return data; 
    }
}

export interface IAuthenticationRequest {
    username?: string | undefined;
    password?: string | undefined;
}

export class ClientDto implements IClientDto {
    clientId?: number;
    deleted?: boolean;
    userId?: number;
    dateCreatedUtc?: Date;
    dateModifiedUtc?: Date | undefined;
    clientName?: string | undefined;
    clientEmail?: string | undefined;

    constructor(data?: IClientDto) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.clientId = _data["clientId"];
            this.deleted = _data["deleted"];
            this.userId = _data["userId"];
            this.dateCreatedUtc = _data["dateCreatedUtc"] ? new Date(_data["dateCreatedUtc"].toString()) : <any>undefined;
            this.dateModifiedUtc = _data["dateModifiedUtc"] ? new Date(_data["dateModifiedUtc"].toString()) : <any>undefined;
            this.clientName = _data["clientName"];
            this.clientEmail = _data["clientEmail"];
        }
    }

    static fromJS(data: any): ClientDto {
        data = typeof data === 'object' ? data : {};
        let result = new ClientDto();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["clientId"] = this.clientId;
        data["deleted"] = this.deleted;
        data["userId"] = this.userId;
        data["dateCreatedUtc"] = this.dateCreatedUtc ? this.dateCreatedUtc.toISOString() : <any>undefined;
        data["dateModifiedUtc"] = this.dateModifiedUtc ? this.dateModifiedUtc.toISOString() : <any>undefined;
        data["clientName"] = this.clientName;
        data["clientEmail"] = this.clientEmail;
        return data; 
    }
}

export interface IClientDto {
    clientId?: number;
    deleted?: boolean;
    userId?: number;
    dateCreatedUtc?: Date;
    dateModifiedUtc?: Date | undefined;
    clientName?: string | undefined;
    clientEmail?: string | undefined;
}

export abstract class BaseApiRequest implements IBaseApiRequest {
    user?: UserDto | undefined;
    userId?: number;

    constructor(data?: IBaseApiRequest) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.user = _data["user"] ? UserDto.fromJS(_data["user"]) : <any>undefined;
            this.userId = _data["userId"];
        }
    }

    static fromJS(data: any): BaseApiRequest {
        data = typeof data === 'object' ? data : {};
        throw new Error("The abstract class 'BaseApiRequest' cannot be instantiated.");
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["user"] = this.user ? this.user.toJSON() : <any>undefined;
        data["userId"] = this.userId;
        return data; 
    }
}

export interface IBaseApiRequest {
    user?: UserDto | undefined;
    userId?: number;
}

export class DerivedBaseApiRequest extends BaseApiRequest implements IDerivedBaseApiRequest {
    test?: string | undefined;

    constructor(data?: IDerivedBaseApiRequest) {
        super(data);
    }

    init(_data?: any) {
        super.init(_data);
        if (_data) {
            this.test = _data["test"];
        }
    }

    static fromJS(data: any): DerivedBaseApiRequest {
        data = typeof data === 'object' ? data : {};
        let result = new DerivedBaseApiRequest();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["test"] = this.test;
        super.toJSON(data);
        return data; 
    }
}

export interface IDerivedBaseApiRequest extends IBaseApiRequest {
    test?: string | undefined;
}

export class UserDto implements IUserDto {
    userId?: number;
    deleted?: boolean;
    dateCreatedUtc?: Date;
    dateModified?: Date | undefined;
    lastLoginDate?: Date | undefined;
    firstName?: string | undefined;
    lastName?: string | undefined;
    username?: string | undefined;
    userEmail?: string | undefined;

    constructor(data?: IUserDto) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.userId = _data["userId"];
            this.deleted = _data["deleted"];
            this.dateCreatedUtc = _data["dateCreatedUtc"] ? new Date(_data["dateCreatedUtc"].toString()) : <any>undefined;
            this.dateModified = _data["dateModified"] ? new Date(_data["dateModified"].toString()) : <any>undefined;
            this.lastLoginDate = _data["lastLoginDate"] ? new Date(_data["lastLoginDate"].toString()) : <any>undefined;
            this.firstName = _data["firstName"];
            this.lastName = _data["lastName"];
            this.username = _data["username"];
            this.userEmail = _data["userEmail"];
        }
    }

    static fromJS(data: any): UserDto {
        data = typeof data === 'object' ? data : {};
        let result = new UserDto();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["userId"] = this.userId;
        data["deleted"] = this.deleted;
        data["dateCreatedUtc"] = this.dateCreatedUtc ? this.dateCreatedUtc.toISOString() : <any>undefined;
        data["dateModified"] = this.dateModified ? this.dateModified.toISOString() : <any>undefined;
        data["lastLoginDate"] = this.lastLoginDate ? this.lastLoginDate.toISOString() : <any>undefined;
        data["firstName"] = this.firstName;
        data["lastName"] = this.lastName;
        data["username"] = this.username;
        data["userEmail"] = this.userEmail;
        return data; 
    }
}

export interface IUserDto {
    userId?: number;
    deleted?: boolean;
    dateCreatedUtc?: Date;
    dateModified?: Date | undefined;
    lastLoginDate?: Date | undefined;
    firstName?: string | undefined;
    lastName?: string | undefined;
    username?: string | undefined;
    userEmail?: string | undefined;
}

export class WeatherForecast implements IWeatherForecast {
    date?: Date;
    temperatureC?: number;
    temperatureF?: number;
    summary?: string | undefined;

    constructor(data?: IWeatherForecast) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.date = _data["date"] ? new Date(_data["date"].toString()) : <any>undefined;
            this.temperatureC = _data["temperatureC"];
            this.temperatureF = _data["temperatureF"];
            this.summary = _data["summary"];
        }
    }

    static fromJS(data: any): WeatherForecast {
        data = typeof data === 'object' ? data : {};
        let result = new WeatherForecast();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["date"] = this.date ? this.date.toISOString() : <any>undefined;
        data["temperatureC"] = this.temperatureC;
        data["temperatureF"] = this.temperatureF;
        data["summary"] = this.summary;
        return data; 
    }
}

export interface IWeatherForecast {
    date?: Date;
    temperatureC?: number;
    temperatureF?: number;
    summary?: string | undefined;
}

export class SwaggerException extends Error {
    message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isSwaggerException = true;

    static isSwaggerException(obj: any): obj is SwaggerException {
        return obj.isSwaggerException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): Observable<any> {
    if (result !== null && result !== undefined)
        return _observableThrow(result);
    else
        return _observableThrow(new SwaggerException(message, status, response, headers, null));
}

function blobToText(blob: any): Observable<string> {
    return new Observable<string>((observer: any) => {
        if (!blob) {
            observer.next("");
            observer.complete();
        } else {
            let reader = new FileReader();
            reader.onload = event => {
                observer.next((<any>event.target).result);
                observer.complete();
            };
            reader.readAsText(blob);
        }
    });
}