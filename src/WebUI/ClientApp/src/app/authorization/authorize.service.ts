import { Injectable } from '@angular/core';
import { Log, User, UserManager, WebStorageStateStore } from 'oidc-client';
import { BehaviorSubject, concat, from, Observable, of, Subscription, timer } from 'rxjs';
import { filter, map, mergeMap, take, tap } from 'rxjs/operators';
import { ApplicationPaths, ApplicationName } from './api-authorization.constants';
import jwt_decode from 'jwt-decode';
import { ApplicationUserDto, LoginResponse } from '../web-api-client';
import { Router } from '@angular/router';

export interface IUser {
  name: string;
}
@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {
  private currentUser: ApplicationUserDto;
  private token: string;
  private userSubject: BehaviorSubject<ApplicationUserDto | null> = new BehaviorSubject(null);
  private source = timer(1000, 1000);
  private sectionSubscribe: Subscription;

  constructor (
    private router: Router
  ) { }

  public saveUserLogin(user: LoginResponse) {
    this.currentUser = user.user;
    this.token = user.token;
    localStorage.setItem('currentUser', JSON.stringify(this.currentUser));
    localStorage.setItem('token', JSON.stringify(this.token));
  }

  public getUser(): Observable<ApplicationUserDto | null> {
    return this.getUserFromStorage();
  }

  public getAccessToken(): Observable<string> {
    if (!this.token) {
      this.token = JSON.parse(localStorage.getItem('token'));
    }
    return of(this.token);
  }

  public isAuthenticated(): Observable<boolean> {
    let isExpired = false;
    this.getAccessToken()
      .subscribe(token => {
        if (token) {
          let val = this.getDecodedAccessToken(token);
          if (this.tokenExpired(val)) {
            isExpired = true;
            localStorage.removeItem('currentUser');
            localStorage.removeItem('token');
          }
        }
      });

    return this.getUser().pipe(map(u => !!u && !isExpired));
  }

  public logout() {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('token');
    this.currentUser = null;
    this.navigateToUrl(ApplicationPaths.Login);
  }

  public async navigateToUrl(returnUrl: string) {
    await this.router.navigateByUrl(returnUrl, {
      replaceUrl: true
    });
  }

  public async navigateToUrlWithoutReplace(returnUrl: string) {
    await this.router.navigateByUrl(returnUrl, {
      replaceUrl: false
    });
  }

  public async navigateToUrlWithParam(returnUrl: string, param) {
    await this.router.navigate([returnUrl], {
      queryParams: param
    });
  }

  private getUserFromStorage(): Observable<ApplicationUserDto> {
    if (!this.currentUser) {
      this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    }
    return of(this.currentUser);
  }

  private tokenExpired(token) {
    return (Math.floor((new Date).getTime() / 1000)) >= token?.exp;
  }

  private getDecodedAccessToken(token: string): any {
    try {
      return jwt_decode(token);
    }
    catch (Error) {
      return null;
    }
  }

  public startTimeoutTimer() {
    if (this.sectionSubscribe && !this.sectionSubscribe.closed) {
      this.sectionSubscribe.unsubscribe();
    }

    if (this.router.url === '/timeout') {
      return;
    }

    this.sectionSubscribe = this.source.subscribe(val => {
      if (this.currentUser !== null && val === this.currentUser.webSessionTimeout) {
        this.sectionSubscribe.unsubscribe();
        localStorage.removeItem('currentUser');
        localStorage.removeItem('token');
        localStorage.removeItem('data');
        this.navigateToUrl('/timeout');
      }
    });
  }
}
