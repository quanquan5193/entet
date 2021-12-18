import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';

import { GetTokenQuery, LoginClient } from 'src/app/web-api-client';
import { AuthorizeService } from '../authorize.service';
import * as dayjs from 'dayjs';
import { CONFIG } from 'src/app/share/constants/config.constants';
import { SERVER_ERROR } from 'src/app/share/constants/error.constants';

// The main responsibility of this component is to handle the user's login process.
// This is the starting point for the login process. Any component that needs to authenticate
// a user can simply perform a redirect to this component with a returnUrl query parameter and
// let the component perform the login and return back to the return url.
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  @ViewChild('password') passwordRef: ElementRef;
  validateForm!: FormGroup;
  returnUrl: string;
  message = '';
  check: Observable<boolean>;
  user: Observable<any>;

  constructor(
    private fb: FormBuilder,
    private loginClient: LoginClient,
    private authService: AuthorizeService,
    private activatedRoute: ActivatedRoute,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.validateForm = this.fb.group({
      userName: [null, [Validators.required]],
      password: [null, [Validators.required]],
      remember: [true]
    });
    this.returnUrl = this.activatedRoute.snapshot.queryParams?.returnUrl ? this.activatedRoute.snapshot.queryParams?.returnUrl : null;
    this.authService.isAuthenticated()
      .subscribe(isAuthenticated => {
        if (isAuthenticated) this.authService.navigateToUrl('/');
      });
    this.validateForm.controls.userName.markAsUntouched();
    this.validateForm.controls.password.markAsUntouched();
  }

  submitForm(): void {
    let param = new GetTokenQuery();
    param.email = this.validateForm.controls.userName.value;
    param.password = this.validateForm.controls.password.value;
    param.isWebApp = true;
    this.loginClient.create(param).subscribe(res => {
      this.authService.saveUserLogin(res);
      if (this.returnUrl) {
        this.authService.navigateToUrl(this.returnUrl);
      } else {
        this.authService.navigateToUrl('/');
      }
    },
      error => {
        this.onError(error);
        this.validateForm.controls.password.setValue('');
        this.validateForm.controls.password.markAsTouched();
      }
    );
  }

  private onError(msg) {
    if (msg && msg.status == SERVER_ERROR.NotFound) {
      this.message = this.translate.instant('authorize.userNotFound');
    } else {
      this.message = this.translate.instant('authorize.unknownException');
    }
  }
}
