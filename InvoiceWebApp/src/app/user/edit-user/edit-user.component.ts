import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import Role from '../../shared/models/role.model';
import User from '../../shared/models/user.model';
import { RoleService } from '../../shared/services/role.service';
import { UserService } from '../../shared/services/user.service';

@Component({
    selector: 'app-edit-user',
    templateUrl: './edit-user.component.html',
    styleUrls: ['./edit-user.component.scss']
})
export class EditUserComponent implements OnInit {
    email: string;
    user: User;
    currentUser: User;
    roles: Role[] = null;

    @ViewChild('fileInput') fileInput: ElementRef;

    constructor(private titleService: Title, private route: ActivatedRoute, private userService: UserService, private roleService: RoleService, private router: Router) { }

    ngOnInit() {
        this.titleService.setTitle('Edit User - Invoice Panel');
        this.getCurrentUser();
        this.route.params.subscribe(
            (params) => {
                this.email = params['email'];
                this.getRoles();
                this.getUser(this.email);
            }
        );
    }

    getRoles() {
        this.roleService.getAll().subscribe(
            (response) => this.roles = response,
            (error) => { throw error; }
        );
    }

    submitForm() {
        this.userService.update(this.user).subscribe(
            (response) => {
                if (response != null) {
                    this.fileUpload();
                }
            },
            (error) => { throw error; }
        );
    }

    getUser(email: string) {
        this.userService.getByEmail(email).subscribe(
            (response) => this.user = response,
            (error) => { throw error; }
        );
    }

    getCurrentUser() {
        this.currentUser = JSON.parse(sessionStorage.getItem('loggedInUser'));
    }

    fileUpload(): void {
        let fi = this.fileInput.nativeElement;
        if (fi.files && fi.files[0]) {
            let fileToUpload = fi.files[0];

            if (fileToUpload) {
                this.userService.upload(fileToUpload, this.user).subscribe(
                    (response) => this.router.navigate(['/users']),
                    (error) => { throw error; }
                );
            }
        } else {
            this.router.navigate(['/users']);
        }
    }
}
