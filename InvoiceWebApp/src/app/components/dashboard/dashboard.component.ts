import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import User from '../../shared/models/user.model';
import { InvoiceService } from '../../shared/services/invoice.service';
import { UserService } from '../../shared/services/user.service';
import Settings from '../../shared/models/settings.model';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
    settings: Settings = JSON.parse(sessionStorage.getItem('settings'));
    currentUser: User = JSON.parse(sessionStorage.getItem('signedInUser'));
    invoice_count: number = 0;
    show: boolean = false;

    constructor(private titleService: Title, private userService: UserService, private invoiceService: InvoiceService) { }

    ngOnInit() {
        this.titleService.setTitle('Home - ' + this.settings.company_name);
        this.getUnpaidInvoiceCount();
    }

    getUnpaidInvoiceCount() {
        this.invoiceService.getAll().subscribe(
            (response) => this.invoice_count = response.filter(f => f.is_paid == false).length,
            (error) => { throw error; }
        );
    }
}
