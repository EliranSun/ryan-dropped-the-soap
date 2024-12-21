def create_branch_name(input_string):
    branch_name = input_string.replace(": ", "-").replace(" ", "-")
    return branch_name


if __name__ == "__main__":
    input_string = input("Enter ticket title: ")
    branch_name = create_branch_name(input_string)
    print(branch_name)
