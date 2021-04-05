<template>
    <b-container class="categoryList">
        <b-row>
            <b-col>
                <b-table 
                    striped 
                    hover 
                    :items="categories" 
                    isBusy="isLoading" 
                    :fields="fields" 
                    :select-mode="'single'"
                    selectable
                    @row-selected="onRowSelected"
                    outlined>
                </b-table>
            </b-col>
        </b-row>
        <b-row v-if="isSelected">
            <b-col>
                <b-container class="border p-2 mb-3">
                    <p>Selected "{{ selected.name }}"</p>
                    <small>Questions: {{ selectedQuestions }}</small>
                </b-container>
            </b-col>
        </b-row>
        <b-row>
            <b-col>
                <b-button-group>
                    <b-button variant="success" @click="onCreate">New</b-button>
                    <b-button :disabled="!isSelected" variant="primary" @click="onEdit">Edit</b-button>
                    <b-button :disabled="!isSelected" variant="danger" @click="onDelete">Delete</b-button>
                </b-button-group>
            </b-col>
        </b-row>
        <b-modal :id="newModalId" :ref="newModalId" title="New Category" v-on:ok="onButtonEventCreate" v-on:cancel="onButtonEventCreate" v-on:close="onButtonEventCreate">
            <b-container>
                <b-form>
                    <b-form-group label="Category" label-for="model_name">
                        <b-form-input id="model_name" v-model="model.name" required/>
                    </b-form-group>
                </b-form>
            </b-container>
        </b-modal>
        <b-modal :id="editModalId" :ref="editModalId" :title="editModalTitle" cancel-title="reset" v-on:ok="onButtonEventEdit" v-on:cancel="onButtonEventEdit" v-on:close="onButtonEventEdit">
            <b-container>
                <b-form>
                    <b-form-group label="Id" label-for="model_id">
                        <b-form-input id="model_id" v-model="model.id" disabled/>
                    </b-form-group>
                    <b-form-group label="Questions" label-for="model_questions">
                        <b-form-input id="model_questions" v-model="modelQuestions" disabled/>
                    </b-form-group>
                    <b-form-group label="Category" label-for="model_name">
                        <b-form-input id="model_name" v-model="model.name" required/>
                    </b-form-group>
                </b-form>
            </b-container>
        </b-modal>
    </b-container>
</template>
<script lang="ts">

    import { Category, nameof } from "@/ResponseTypes";
import AuthApi from "@/services/AuthApi";
    import { BvModalEvent } from "node_modules/bootstrap-vue/src/components/modal";
import { NavigationGuardNext, Route } from "node_modules/vue-router/types/router";
    import { Component, Vue } from 'vue-property-decorator';
    import CategoryCrud from "../CRUD/CategoryCrud";

    @Component({
        components: {
            
        }
    })
    export default class QuestionList extends Vue {
        private selected: Category = new Category();
        private warningText: string = '';
        private categories: Category[] = [];
        private isLoading: boolean = true;

        private model: Category = new Category();

        private fields = [
            { key: nameof<Category>("id"),   label: nameof<Category>("id").toUpperCase() },
            { key: nameof<Category>("name"), label: nameof<Category>("name").toUpperCase() },
        ]

        async mounted() {
            await this.reload()
        }

        async reload() {
            this.isLoading = true;
            this.categories = await CategoryCrud.getAll();
            this.isLoading = false;
        }

        private readonly newModalId = "modal-category-new"
        private readonly editModalId = "modal-category-edit";

        onCreate() {
            this.model = new Category();

            this.$bvModal.show(this.newModalId);
        }

        async onButtonEventCreate(evt: BvModalEvent) {
            const trigger: BvModalResponse = evt.trigger as BvModalResponse ?? 'cancel';

            if(trigger == 'ok') {
                
                if(this.model.name === undefined || this.model.name.trim() === '') {
                    evt.preventDefault();
                } else {
                    await CategoryCrud.create(this.model.name);
                    await this.reload();
                }

            } else if(trigger == 'cancel') {
                this.model = new Category();
            } else if(trigger == 'headerclose') {
                this.model = new Category();
            }
        }

        onEdit() {
            if(!this.selected) {
                return;
            }

            //Clone because of references
            this.model = Object.assign(new Category(), this.selected);

            this.$bvModal.show(this.editModalId);
        }

        async onButtonEventEdit(evt: BvModalEvent) {
            const trigger: BvModalResponse = evt.trigger as BvModalResponse ?? 'cancel';
            if(trigger == 'ok') {
                
                if(this.model.name === undefined || this.model.name.trim() === '') {
                    evt.preventDefault();
                } else {
                    try {
                        await CategoryCrud.update(this.model.id, this.model.name);
                        this.selected.name = this.model.name;
                        await this.reload();
                    } catch (error) {
                        evt.preventDefault();
                    }
                }

            } else if(trigger == 'cancel') {
                //Here: reset
                this.model.name = this.selected.name;
                evt.preventDefault();
            } else if(trigger == 'headerclose') {
                this.model.name = this.selected.name;
            }
        }

        async onDelete() {
            if(!this.isSelected)
            {
                return;
            }

            const choice = await this.$bvModal.msgBoxConfirm(`Do you really want to delete the category "${this.selected.name}"? In turn, this would delete ${this.selected.questions.$values.length} ${this.selected.questions.$values.length == 1 ? 'question' : 'questions'}`, {
                headerBgVariant: 'danger',
                okTitle: 'Delete',
                okVariant: 'danger',

            }) ?? false;

            if(!choice) {
                return;
            }

            try {
                let deletedCategoryName = this.selected.name;
                await CategoryCrud.delete(this.selected.id);
                this.selected = new Category();
                
                this.$bvToast.toast(`Category "${deletedCategoryName}" as been deleted successfully!`, {
                    variant: 'success',
                    title: 'Success',
                    solid: true
                })
            } catch (error) {
                this.$bvToast.toast(`Category "${this.selected.name}" could not be deleted!`, {
                    variant: 'danger',
                    title: 'Error',
                    solid: true
                })
            }

            await this.reload();
        }

        get editModalTitle() {
            return `Edit ${this.selected.name}`;
        }

        async onRowSelected(item: Category): Promise<void> {
            if(Array.isArray(item)) {
                item = item[0];
            }

            this.selected = item;
        }

        get modelQuestions() {
            return this.model?.questions?.$values?.length ?? 0;
        }

        get selectedQuestions() {
            return this.selected?.questions?.$values?.length ?? 0;
        }

        get isSelected() {
            return this.selected.id !== undefined;
        }

        public static async beforeEnter(to: Route, 
                                        from: Route, 
                                        next: NavigationGuardNext<Vue>) {
            let isAuthenticated = false; 
            try {
               isAuthenticated = await AuthApi.IsLoggedIn();
            } catch (error) { }
            if(!isAuthenticated) next('/')
            else next()
        }
    }

    type BvModalResponse = 'ok' | 'cancel' | 'headerclose';

</script>
<style scoped>

</style>