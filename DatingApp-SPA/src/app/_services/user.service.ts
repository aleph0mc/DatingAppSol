import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { User } from '../_models/user';
import { Observable } from 'rxjs';
import { PaginatedResult } from '../_models/Pagination';
import { map } from 'rxjs/operators';

//COMMENTED OUT: WE USE THE INJECTION FOR THE TOKEN (SEE app.module.ts)
//const httpOptions = {
//  headers: new HttpHeaders({
//    'Authorization': 'Bearer ' + localStorage.getItem('token')
//  })
//};

@Injectable()
export class UserService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  //REPLACED
  //getUsers(): Observable<User[]> {
  //  //return this.http.get<User[]>(this.baseUrl + 'users', httpOptions);
  //  return this.http.get<User[]>(this.baseUrl + 'users');
  //}
  //WITH THE FOLLOWING FOR PAGINATION
  getUsers(page?: number, itemsPerPage?: number, userParams?: any, likesParam?: string): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    let params = new HttpParams();
    if ((page != null) && (itemsPerPage != null)) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());

      if (null != userParams) {
        params = params.append('minAge', userParams.minAge);
        params = params.append('maxAge', userParams.maxAge);
        params = params.append('gender', userParams.gender);
        params = params.append('orderBy', userParams.orderBy);
      }
    }

    if (likesParam === 'likers')
      params = params.append('likers', 'true');

    if (likesParam === 'likees')
      params = params.append('likees', 'true');

    return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }

          return paginatedResult;
        })
      );
  }


  getUser(id: number): Observable<User> {
    //return this.http.get<User>(this.baseUrl + 'users/' + id, httpOptions);
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

  sendLike(id: number, recepientId: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recepientId, {});
  }
}
