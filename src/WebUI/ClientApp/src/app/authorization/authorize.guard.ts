import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, Route } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthorizeService } from './authorize.service';
import { tap } from 'rxjs/operators';
import { ApplicationPaths, QueryParameterNames } from './api-authorization.constants';
import { PermissionEnum } from '../share/constants/enum.constants';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeGuard implements CanActivate {
  constructor (private authorize: AuthorizeService, private router: Router) {
  }
  canActivate(
    _next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this.authorize.isAuthenticated()
      .pipe(tap(isAuthenticated => this.handleAuthorization(isAuthenticated, state, _next)));
  }

  private handleAuthorization(isAuthenticated: boolean, state: RouterStateSnapshot, route: ActivatedRouteSnapshot) {
    if (!isAuthenticated) {
      this.router.navigate(ApplicationPaths.LoginPathComponents, {
        queryParams: {
          [QueryParameterNames.ReturnUrl]: state.url
        }
      });
    } else {
      const currentUser = JSON.parse(localStorage.getItem("currentUser"));

      if (route.data.funcId !== undefined && currentUser['rolePermission'][route.data.funcId] == PermissionEnum.Forbidden) {
        this.router.navigate(['/access-denied']);
      }
    }

  }
}
