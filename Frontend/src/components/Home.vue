<template>
    <div class="home">
        <h1>{{ msg }}</h1>
        <p>Welcomee to your new single-page application, built with <a href="https://vuejs.org" target="_blank">Vue.js</a> and <a href="http://www.typescriptlang.org/" target="_blank">TypeScript</a>.</p>
        <b-table 
            striped 
            hover 
            :items="categories" 
            isBusy="isLoading" 
            :fields="fields" 
            :select-mode="'single'"
            selectable
            @row-selected="onRowSelected">
        </b-table>
        <b-modal id="modal" ref="modal" :title="'Category'" @ok="onSubmit">
            <b-form>
                <b-form-group>
                    <label for="inputName">Category name</label>
                    <b-form-input :state="state" id="inputName" v-model="selected.name" />
                </b-form-group>
                <b-form-group>
                    <b-btn variant="danger" @click="onDelete">Delete</b-btn>
                </b-form-group>
            </b-form>
        </b-modal>
    </div>
</template>

<script lang="ts"> 
    import { Category, HighScore, nameof, typedUnitialized } from '@/ResponseTypes';
    import { BModal, BNavForm, BvModal, BvModalEvent } from 'bootstrap-vue';
    import { Component, Prop, Vue } from 'vue-property-decorator';
    import ApiExplorer from "./ApiExplorer.vue";
    import CategoryCrud from "./CRUD/CategoryCrud"

    @Component({
        components: {
            ApiExplorer
        }
    })
    export default class Home extends Vue {
        @Prop() private msg!: string;
        private categories: Category[] = [];
        private selected: Category = new Category();
        private fields = [
            { key: nameof<Category>("id"),   label: nameof<Category>("id").toUpperCase() },
            { key: nameof<Category>("name"), label: nameof<Category>("name").toUpperCase() },
        ]
        private isLoading: boolean = true;
        private state: boolean = true;

        mounted() {
            this.loadData();
        }


        show(item: any, index: any, button: any) {
            this.$root.$emit('bv::show::modal', this.selected.$id, button)
        }

        loadData() {
            this.isLoading = true;
            CategoryCrud.getAll()
                        .then(res => this.categories = res)
                        .catch(err => console.error(err))
                        .finally(() => this.isLoading = false)
        }

        onRowSelected(item: Category) {
            console.log(item)
            if(Array.isArray(item)) {
                item = item[0];
            }

            this.selected = item;
            this.modal.show()
        }

        async onSubmit(evt: BvModalEvent) {
            evt.preventDefault();

            await CategoryCrud.update(this.selected.id, this.selected.name);
            this.loadData();

            this.$nextTick(() => {
                this.$bvModal.hide('modal');
            })
        }

        async onDelete(evt: any) {
            await CategoryCrud.delete(this.selected.id);
            this.loadData();

            this.$nextTick(() => {
                this.$bvModal.hide('modal');
            })
        }

        get modal() {
            let m = this.$refs['modal'];

            return m as BModal;
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
